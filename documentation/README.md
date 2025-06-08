El informe de PowerBi se encuentra en el archivo **InformeChokart.pbix**.
Otros datos importantes sobre el proyecto son:

# COPYRIGHT
- Modelo 3D bala de jinx (proyectil): https://sketchfab.com/3d-models/jinxs-bullet-5e6a98a547ba4bccb63a687a6d56d4df

# INSTRUCCIONES PARA EL DESARROLLO
**MUY IMPORTANTE:** Poner todos los vídeos antes de abrir Unity, tal y como estaban, y luego borrarlos al cerrar cuando se vaya a subir algo al repo
- Para los colliders, ponerlos como un GameObject hijo, y que luego el padre tenga el tag y el script con las propiedades (seguramente esto se pueda hacer mejor xD)
- Cuando se cree un clon, poner dentro de la carpeta de este un ".gitignore" con un asterisco. LOS CLONES COMPARTEN PREFERENCIAS (aparantemente)
- Para mandar objetos a través de RPCs, hay que definir campos con \[SerializableField\]
- **IMPORTANTE (BORRAR MÁS TARDE):** Tal y como está ahora (a 23/03) las posiciones parece que las pone mal, pero es porque no hay suficientes triggers (ahora mismo hay 2) y piensa que la distancia hasta el siguiente (que es la línea de meta) es menor dando marcha atrás (obviamente) en lugar de seguir para adelante. Lo que quiero decir con esto es que las posiciones funcionan xD
- El host es el encargado de hacer todo. ¿Spawnear un objeto? Host ¿Borrar un objeto? Host ¿Modificar objetos? Host. Por tanto los clientes invocan, con previa verificación de IsOwner, a un método ServerRPC, y este comunica a otros clientes si hace falta. Recomiendo ver tutoriales de esto porque es muy importante
- Para la IA de los contrincantes, bajar el objeto "grass" un poco antes de darle a "Bake", y luego volverlo a subir con Ctrl + Z
- Para las lobbies no vale usar ParrelSync, sino el nuevo sistema de Multiplayer Player (o algo así) oficial de Unity, dentro de Window/Multiplayer
