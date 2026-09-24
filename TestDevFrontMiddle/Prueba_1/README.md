## Directorio para la prueba 1 ##

Aplicación en React (con Hooks) que consume la API de [JSONPlaceholder](https://jsonplaceholder.typicode.com/guide/). El estado global se maneja con Redux Toolkit y la interfaz con Material UI.

### Cómo ejecutarla

Requiere Node.js 18 o superior.

```bash
cd Prueba_1
npm install
npm run dev
```

La aplicación queda en `http://localhost:5173`.

Otros comandos:

```bash
npm test          # pruebas del estado (Vitest)
npm run build     # compilación de producción
```

### Qué hace

1. Al abrir la página se piden los usuarios a `/users` y se guardan los 10 primeros en el store de Redux.
2. Al seleccionar un usuario se muestran sus datos y los botones **Posts** y **Todos**.
3. **Posts**: una acción de Redux trae `/users/{id}/posts` y, para cada publicación, sus comentarios desde `/posts/{id}/comments`. Los comentarios quedan anidados dentro de cada post.
4. **Todos**: una acción de Redux trae `/users/{id}/todos` y las ordena por `id` de mayor a menor.
5. En la sección de tareas hay un formulario (título, completada y botón Guardar) que hace un `POST` a `/todos` con `{ userId, title, completed }`. El API no guarda nada y responde siempre con `id: 201`; al recibirlo la tarea se agrega a la lista.

### Estructura

```
src/
  api/          jsonPlaceholderApi.js   acceso al API
  store/        un slice por recurso (usuarios, posts, todos)
  components/   ListaUsuarios, DetalleUsuario, ListaPosts, SeccionTodos, FormularioTodo
```

Patrón de diseño: las llamadas HTTP están aisladas en `jsonPlaceholderApi.js` (patrón repositorio), de modo que los slices no dependen de `fetch` ni de las URLs y se pueden probar simulando solo `fetch`.
