import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { jsonPlaceholderApi } from '../api/jsonPlaceholderApi';

export const cargarUsuarios = createAsyncThunk('usuarios/cargar', async () => {
  const usuarios = await jsonPlaceholderApi.obtenerUsuarios();
  return usuarios.slice(0, 10);
});

const usuariosSlice = createSlice({
  name: 'usuarios',
  initialState: {
    items: [],
    seleccionadoId: null,
    estado: 'inactivo',
    error: null,
  },
  reducers: {
    seleccionarUsuario(state, action) {
      state.seleccionadoId = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(cargarUsuarios.pending, (state) => {
        state.estado = 'cargando';
        state.error = null;
      })
      .addCase(cargarUsuarios.fulfilled, (state, action) => {
        state.estado = 'listo';
        state.items = action.payload;
      })
      .addCase(cargarUsuarios.rejected, (state, action) => {
        state.estado = 'error';
        state.error = action.error.message;
      });
  },
});

export const { seleccionarUsuario } = usuariosSlice.actions;

export const seleccionarUsuarioActual = (state) =>
  state.usuarios.items.find((u) => u.id === state.usuarios.seleccionadoId) ?? null;

export default usuariosSlice.reducer;
