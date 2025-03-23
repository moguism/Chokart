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

# INSTRUCCIONES PARA EL DESARROLLO
- Para los colliders, ponerlos como un GameObject hijo, y que luego el padre tenga el tag y el script con las propiedades (seguramente esto se pueda hacer mejor xD)
- Para spawnear un coche sin peer to peer y que le siga la cámara, poner un "NotNetworkPlayer" (comentando las líneas con "IsOwner") y la cámara, en las propiedades "Look At" y "Follow", asignarle el Kart
- Cuando se cree un clon, poner dentro de la carpeta de este un ".gitignore" con un asterisco
- Para mandar objetos a través de RPCs, hay que definir campos con \[SerializableField\]
- **IMPORTANTE (BORRAR MÁS TARDE):** Tal y como está ahora (a 23/03) las posiciones parece que las pone mal, pero es porque no hay suficientes triggers (ahora mismo hay 2) y piensa que la distancia hasta el siguiente (que es la línea de meta) es menor dando marcha atrás (obviamente) en lugar de seguir para adelante. Lo que quiero decir con esto es que las posiciones funcionan xD

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
    Viernes --> Mauricio termina el P2P básico.
    Sábado --> Mauricio programa el cálculo de las posiciones.
    Domingo --> Mauricio termina las posiciones de los coches y el número de vueltas.
