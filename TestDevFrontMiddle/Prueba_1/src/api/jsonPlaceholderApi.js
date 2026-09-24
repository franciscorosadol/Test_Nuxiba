const BASE_URL = 'https://jsonplaceholder.typicode.com';

async function solicitar(ruta, opciones) {
  const respuesta = await fetch(`${BASE_URL}${ruta}`, opciones);
  if (!respuesta.ok) {
    throw new Error(`La petición a ${ruta} falló (${respuesta.status})`);
  }
  return respuesta.json();
}

// Repositorio que concentra todas las llamadas al API. Los slices de Redux
// solo conocen esta interfaz y no saben nada de fetch ni de las URLs.
export const jsonPlaceholderApi = {
  obtenerUsuarios: () => solicitar('/users'),

  obtenerPostsDeUsuario: (userId) => solicitar(`/users/${userId}/posts`),

  obtenerComentariosDePost: (postId) => solicitar(`/posts/${postId}/comments`),

  obtenerTodosDeUsuario: (userId) => solicitar(`/users/${userId}/todos`),

  crearTodo: (todo) =>
    solicitar('/todos', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json; charset=UTF-8' },
      body: JSON.stringify(todo),
    }),
};
