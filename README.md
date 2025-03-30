# DOCUMENTACIÓN
- [Anteproyecto](Anteproyecto.md)

# BIBLIOGRAFÍA
- **Bindings:** https://docs.unity3d.com/Manual/class-InputManager.html
- **Movimiento del coche:** https://www.youtube.com/watch?v=Ki-tWT50cEQ&list=PL1R2qsKCcUCKY1p7URUct96O0dorgQnO6
- **Importar modelos 3D:** https://www.youtube.com/watch?v=CVz2a0Orl_M
- **Texturas y materiales:** https://www.youtube.com/watch?v=yuLcssPwGLc&t=355s
- **Tutorial genérico:** https://www.youtube.com/watch?v=x3FbFa843Pw&t=171s
- **Colisiones:** https://www.youtube.com/watch?v=mkErt53EEFY
- **Botones:** https://www.youtube.com/watch?v=0y719UVB7jQ
- **Exportar a Android:** https://www.youtube.com/watch?v=RIYIcAcS5IA
- **Websockets:** https://github.com/endel/NativeWebSocket
- **Servidor UDP:** https://www.youtube.com/watch?v=8w97YdWNLHA&ab_channel=v11tv
- **Servidor UDP:** https://gist.github.com/darkguy2008/413a6fea3a5b4e67e5e0d96f750088a9
- **Multijugador P2P:** https://www.youtube.com/watch?v=dUqLShvBIsM, https://www.youtube.com/watch?v=2OLUdPkkQPI, https://www.youtube.com/watch?v=3yuBOB3VrCk&t=198s
- **Cálculo de posiciones:** https://www.youtube.com/watch?v=fdWjR652Fs4
- **Login y registro:** https://youtu.be/H_WDhi8oXpg?si=z-ryro65byue_6yT
- **Extraer IP del cliente** https://blog.elmah.io/how-to-get-the-client-ip-in-asp-net-core-even-behind-a-proxy/
- **IA de los contrincantes** https://www.youtube.com/watch?v=SMWxCpLvrcc
- **Menú de selección** https://www.youtube.com/watch?v=tYK-rbuMF1k

# INSTRUCCIONES PARA EL DESARROLLO
- Para los colliders, ponerlos como un GameObject hijo, y que luego el padre tenga el tag y el script con las propiedades (seguramente esto se pueda hacer mejor xD)
- Cuando se cree un clon, poner dentro de la carpeta de este un ".gitignore" con un asterisco. LOS CLONES COMPARTEN PREFERENCIAS (aparantemente)
- Para mandar objetos a través de RPCs, hay que definir campos con \[SerializableField\]
- **IMPORTANTE (BORRAR MÁS TARDE):** Tal y como está ahora (a 23/03) las posiciones parece que las pone mal, pero es porque no hay suficientes triggers (ahora mismo hay 2) y piensa que la distancia hasta el siguiente (que es la línea de meta) es menor dando marcha atrás (obviamente) en lugar de seguir para adelante. Lo que quiero decir con esto es que las posiciones funcionan xD
- El host es el encargado de hacer todo. ¿Spawnear un objeto? Host ¿Borrar un objeto? Host ¿Modificar objetos? Host. Por tanto los clientes invocan, con previa verificación de IsOwner, a un método ServerRPC, y este comunica a otros clientes si hace falta. Recomiendo ver tutoriales de esto porque es muy importante
- Para la IA de los contrincantes, bajar el objeto "grass" un poco antes de darle a "Bake", y luego volverlo a subir con Ctrl + Z

# DIARIO DE DESARROLLO
### Semana de preparativos (10/03 - 16/03)
    Miércoles --> Votamos y sale adelante esta idea.
    Viernes --> Mauricio empieza con el movimiento básico del coche sin mucho éxito, y María investiga el multijugador. Nos familiarizamos con Unity.
    Sábado --> Mauricio termina el movimiento básico (derrape y movimiento hacia delante y a los lados).
    Domingo --> Mauricio agrega la posibilidad de chocar con otros coches y quitarles vida en función de la velocidad al impactar.
### Semana de preparativos (17/03 - 23/03)
    Lunes --> María agrega movimiento de coche con W y S, mostrar salud en pantalla e intenta arreglar fallo de derrape con botón.
    Martes --> Mauricio termina el derrape, que fallaba en móvil al implementar el giroscopio, y crea el servidor básico.
    Miércoles --> María implementa servidor UDP con socket en back. Mauricio prepara los websockets y se pone con el sistema de lobbies.
    Jueves --> Mauricio implementa un sistema P2P de prueba para el multijugador. 
    Viernes --> Mauricio termina el P2P básico. Tutoría y reunión del proyecto.
    Sábado --> Mauricio programa el cálculo de las posiciones.
    Domingo --> Mauricio termina las posiciones de los coches y el número de vueltas. Rocío implementa el login y registro de usuarios.
### Semana de preparativos (24/03 - 30/03)
    Lunes --> Rocío implementa el recuérdame del login, cerrar sesión y el cálculo de velocidad de los coches para el velocímetro.
    Martes --> Mauricio interconecta todo lo que está hecho hasta el momento y hace el matchmaking aleatorio. María busca documentación para hacer la degradación del coche.
    Miércoles --> Mauricio hace el sistema de objetos en función de la posición y el choque con otros coches. María termina de implementar el velocímetro y el cronómetro.
    Jueves --> Turoría de seguimiento y primera llamada con el animador.
    Viernes --> Mauricio mejora el turbo, el movimiento del coche y varios códigos en general, al igual que empieza con la IA de los contrincantes.
    Sábado --> Mauricio continua con la IA de los bots, sin mucho resultado.
    Domingo --> Mauricio hace el menú de selección.
