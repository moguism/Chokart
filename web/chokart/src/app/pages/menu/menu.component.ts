import { Component, OnDestroy, OnInit } from '@angular/core';
import { WebsocketService } from '../../services/websocket.service';
import { Subscription } from 'rxjs';
import { MessageType } from '../../models/message-type';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../services/auth.service';
import { FriendshipService } from '../../services/friendship.service';
import { Friend } from '../../models/friend';
import { CustomRouterService } from '../../services/custom-router.service';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})
export class MenuComponent implements OnInit, OnDestroy {
  messageReceived$: Subscription | null = null;
  serverResponse: string = '';

  user: User | null = null;

  friendsRaw: Friend[] = []
  acceptedFriends: Friend[] = []
  pendingFriends: Friend[] = []

  searchedUsers!: User[];
  searchedFriends: Friend[] = [];
  queryuser: string = '';
  queryfriend: string = '';

  menuSelector: string = 'myFriends';  // myFriends, searchUsers, friendRequest, battleRequest


  public readonly IMG_URL = environment.apiImg;

  constructor(private webSocketService: WebsocketService,
    private router: CustomRouterService, private userService: UserService,
    public authService: AuthService,
    private friendshipService: FriendshipService,
  ) { }

  // TODO: Redirigir al login si no ha iniciado sesión
  async ngOnInit() {

    if(!this.authService.isAuthenticated()){
      this.router.navigateToUrl("login");
    } else {
      // Procesa la respuesta
    this.webSocketService.connectNative()

    this.messageReceived$ = this.webSocketService.messageReceived.subscribe(message => this.processMessage(message))

    this.user = this.authService.getUser()
    await this.getCurrentUser()
    
    console.log("USUARIO: ", this.user)
    }
  }

  async processMessage(message: any) {
    this.serverResponse = message
    const jsonResponse = JSON.parse(this.serverResponse)

    // En función del tipo de mensaje que he recibido, sé que me han enviado unos datos u otros

    // Es posible que haya que hacer JSON.parse() otra vez en alguno de los casos
    switch (jsonResponse.messageType) {
      case MessageType.AskForFriend:
        await this.getCurrentUser()
        break
      case MessageType.FriendUpdate:
        await this.getCurrentUser()
        break
    }
    console.log("Respuesta del socket en JSON: ", jsonResponse)
  }

  async getCurrentUser()
  {
    this.user = await this.userService.getCurrentUser(this.user.id)
    this.authService.saveUser(this.user)
    this.processFriends()
  }

  processFriends() {
    console.log(this.user.friendships)
    this.friendsRaw = this.user.friendships

    this.acceptedFriends = []
    this.pendingFriends = []
    console.log(this.friendsRaw)
    for(const friend of this.friendsRaw)
    {
      if(friend.accepted)
      {
        this.acceptedFriends.push(friend)
      }
      if(friend.accepted === false)
      {
        if(this.user?.id == friend.receiverUserId)
        {
          this.pendingFriends.push(friend)
        }
      }
    }
    console.log("amigos: ", this.acceptedFriends)
    console.log("solicitudes: ", this.pendingFriends)

    this.searchFriend("")
  }

  async removeFriend(friend: Friend) {
    // En el servidor se llamaría a un método para borrar la amistad, ( wesoque ->) el cual llamaría al socket del otro usuario para notificarle
    // Para recibir la notificación ya se encarga "processMesage", y de actualizar la lista

    const nickname = friend.receiverUser?.nickname || friend.senderUser?.nickname;

    const confirmed = window.confirm(`¿Seguro que quieres dejar de ser amigo de ${nickname}?`);
  
    if (confirmed) {
      await this.friendshipService.removeFriendById(friend.id)
      alert(`Has dejado de ser amigo de ${nickname}.`);
    } 
  }

  async addFriend(user: User) {
    // Hago una petición para que cree el amigo, ( wesoque ->) y en back el servidor debería notificar a ambos usuarios enviando la lista de amigos
    const response = await this.friendshipService.addFriend(user)
    console.log("Respuesta de agregar al amigo: ", response)
  }

  async acceptFriendship(id: number) {
    const response = await this.friendshipService.acceptFriendship(id)
    console.log("Respuesta de aceptar al amigo: ", response)
  }

  askForInfo(messageType: MessageType) {
    console.log("Mensaje pedido: ", messageType)
    this.webSocketService.sendNative(messageType.toString())
  }

  ngOnDestroy(): void {
    this.messageReceived$?.unsubscribe();
  }

  visitUserPorfile(user: User | null)
  {
    if(user){
      this.router.navigateToUrl("profile/" +user?.id  );
    }
  }

  async getSearchedUsers(queryuser: string): Promise<User[]> {
    const result = await this.userService.searchUser(queryuser);
    console.log(result)

    this.searchedUsers = result;

    return result;
  }


  searchFriend(queryfriend: string): void {

    const query = this.removeAccents(queryfriend)

    let encontrados: Friend[] = [];
    const misAmigos: User[] = []

    this.acceptedFriends.forEach(friendship => {

      // si el receiver es nulo, busco entre los sender
      if (friendship.senderUser) {

        encontrados = (this.acceptedFriends.filter(user => user.senderUser?.nickname.includes(query)))

        encontrados.forEach(amigo => {
          if (amigo.senderUser) {
            misAmigos.push(amigo.senderUser)
          }
        });

      } else {
        encontrados = this.acceptedFriends.filter(user => user.receiverUser?.nickname.includes(query))
        encontrados.forEach(amigo => {
          if (amigo.receiverUser) {
            misAmigos.push(amigo.receiverUser)
          }
        });
      }
    } 
  );
    // aqui tambien se pueden guardar los usuarios USER de los amigos 
    this.searchedFriends = encontrados;
  }

  // comprueba q el usuatio ya tiene amistad (aceptada o no) con otro usuario
  hasFriendship(user: User): boolean {
    return this.friendsRaw.some(friend =>
      (friend.senderUserId === user.id && friend.receiverUserId === this.user?.id) || 
      (friend.receiverUserId === user.id && friend.senderUserId === this.user?.id)
    );
  }

  // comprueba si se le ha enviado una solicitud de amistad y esta en espera
  waitingFriendship(user: User): boolean {
    const amistad : Friend | undefined= this.friendsRaw.find(friend =>
      (friend.senderUserId === user.id && friend.receiverUserId === this.user?.id) || 
      (friend.receiverUserId === user.id && friend.senderUserId === this.user?.id)
    )
    if(amistad) {
      return !amistad.accepted
    } else return false
  }

  // quita tildes y pone minuscula
  removeAccents(str: string): string {
    return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
  }
}
