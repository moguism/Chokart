import { Component, Input } from '@angular/core';
import { FriendshipService } from '../../services/friendship.service';
import { SweetalertService } from '../../services/sweetalert.service';
import { Friend } from '../../models/friend';
import { User } from '../../models/user';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';

@Component({
  selector: 'app-friend-card',
  standalone: true,
  imports: [TranslocoModule],
  templateUrl: './friend-card.component.html',
  styleUrl: './friend-card.component.css'
})
export class FriendCardComponent {

  @Input() friendName: string = ""
  @Input() canDelete: boolean = true
  @Input() canAdd: boolean = true
  @Input() canAccept: boolean = true
  @Input() user: User
  @Input() friend: Friend

  constructor(private friendshipService: FriendshipService, private sweetAlertService: SweetalertService, private translocoService: TranslocoService) { }

  async removeFriend() {
    // En el servidor se llamaría a un método para borrar la amistad, ( wesoque ->) el cual llamaría al socket del otro usuario para notificarle
    // Para recibir la notificación ya se encarga "processMesage", y de actualizar la lista

    const nickname = this.friend.receiverUser?.nickname || this.friend.senderUser?.nickname;
    const confirmed = window.confirm(`${this.translocoService.translate("remove-friend-sure")} ${nickname}?`);

    if (confirmed) {
      await this.friendshipService.removeFriendById(this.friend.id)
      this.sweetAlertService.showAlert('Info', `${this.translocoService.translate("friend-removed")} ${nickname}.`, 'info');
    }
  }

  async addFriend() {
    // Hago una petición para que cree el amigo, ( wesoque ->) y en back el servidor debería notificar a ambos usuarios enviando la lista de amigos
    const response = await this.friendshipService.addFriend(this.user)
    console.log("Respuesta de agregar al amigo: ", response)
  }

  async acceptFriendship() {
    const response = await this.friendshipService.acceptFriendship(this.friend.id)
    console.log("Respuesta de aceptar al amigo: ", response)
  }

}
