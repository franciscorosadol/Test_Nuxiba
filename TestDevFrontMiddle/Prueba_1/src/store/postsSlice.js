import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { jsonPlaceholderApi } from '../api/jsonPlaceholderApi';
import { seleccionarUsuario } from './usuariosSlice';

const porId = (a, b) => a.id - b.id;

// Trae las publicaciones del usuario y anida en cada una sus comentarios.
// Los textos del API vienen en latín, se reemplazan por textos sencillos en español.
export const cargarPostsDeUsuario = createAsyncThunk('posts/cargarDeUsuario', async (userId) => {
  const posts = await jsonPlaceholderApi.obtenerPostsDeUsuario(userId);
  return Promise.all(
    [...posts].sort(porId).map(async (post, indice) => {
      const comentarios = await jsonPlaceholderApi.obtenerComentariosDePost(post.id);
      return {
        ...post,
        title: `Publicación ${indice + 1}`,
        body: `Este es el contenido de la publicación ${indice + 1}.`,
        comments: [...comentarios].sort(porId).map((comentario, i) => ({
          ...comentario,
          name: `Comentario ${i + 1}`,
          body: `Este es el texto del comentario ${i + 1}.`,
        })),
      };
    }),
  );
});

const estadoInicial = { items: [], estado: 'inactivo', error: null };

const postsSlice = createSlice({
  name: 'posts',
  initialState: estadoInicial,
  extraReducers: (builder) => {
    builder
      .addCase(cargarPostsDeUsuario.pending, (state) => {
        state.estado = 'cargando';
        state.error = null;
      })
      .addCase(cargarPostsDeUsuario.fulfilled, (state, action) => {
        state.estado = 'listo';
        state.items = action.payload;
      })
      .addCase(cargarPostsDeUsuario.rejected, (state, action) => {
        state.estado = 'error';
        state.error = action.error.message;
      })
      // Al cambiar de usuario se descartan las publicaciones del anterior
      .addCase(seleccionarUsuario, () => estadoInicial);
  },
});

export default postsSlice.reducer;
