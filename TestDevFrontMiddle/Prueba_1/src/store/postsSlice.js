import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { jsonPlaceholderApi } from '../api/jsonPlaceholderApi';
import { seleccionarUsuario } from './usuariosSlice';

// Trae las publicaciones del usuario y anida en cada una sus comentarios
export const cargarPostsDeUsuario = createAsyncThunk('posts/cargarDeUsuario', async (userId) => {
  const posts = await jsonPlaceholderApi.obtenerPostsDeUsuario(userId);
  return Promise.all(
    posts.map(async (post) => ({
      ...post,
      comments: await jsonPlaceholderApi.obtenerComentariosDePost(post.id),
    })),
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
