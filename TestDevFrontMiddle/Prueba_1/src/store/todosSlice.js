import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { jsonPlaceholderApi } from '../api/jsonPlaceholderApi';
import { seleccionarUsuario } from './usuariosSlice';

// Ordena las tareas por id de mayor a menor
export const ordenarPorIdDescendente = (todos) => [...todos].sort((a, b) => b.id - a.id);

export const cargarTodosDeUsuario = createAsyncThunk('todos/cargarDeUsuario', async (userId) => {
  const todos = await jsonPlaceholderApi.obtenerTodosDeUsuario(userId);
  // Los títulos del API vienen en latín, se reemplazan por nombres sencillos (Tarea 1, Tarea 2...)
  const conNombre = [...todos]
    .sort((a, b) => a.id - b.id)
    .map((todo, indice) => ({ ...todo, title: `Tarea ${indice + 1}` }));
  return ordenarPorIdDescendente(conNombre);
});

export const agregarTodo = createAsyncThunk('todos/agregar', async ({ userId, title, completed }) => {
  const creado = await jsonPlaceholderApi.crearTodo({ userId, title, completed });
  return { userId, title, completed, id: creado.id };
});

const estadoInicial = { items: [], estado: 'inactivo', error: null, guardando: false, errorGuardado: null };

const todosSlice = createSlice({
  name: 'todos',
  initialState: estadoInicial,
  extraReducers: (builder) => {
    builder
      .addCase(cargarTodosDeUsuario.pending, (state) => {
        state.estado = 'cargando';
        state.error = null;
      })
      .addCase(cargarTodosDeUsuario.fulfilled, (state, action) => {
        state.estado = 'listo';
        state.items = action.payload;
      })
      .addCase(cargarTodosDeUsuario.rejected, (state, action) => {
        state.estado = 'error';
        state.error = action.error.message;
      })
      .addCase(agregarTodo.pending, (state) => {
        state.guardando = true;
        state.errorGuardado = null;
      })
      .addCase(agregarTodo.fulfilled, (state, action) => {
        state.guardando = false;
        // El API siempre responde con id 201 y no guarda nada, así que si ese id ya
        // está en la lista se usa el siguiente disponible para no repetir llaves
        const repetido = state.items.some((t) => t.id === action.payload.id);
        const id = repetido ? Math.max(...state.items.map((t) => t.id)) + 1 : action.payload.id;
        state.items = ordenarPorIdDescendente([...state.items, { ...action.payload, id }]);
      })
      .addCase(agregarTodo.rejected, (state, action) => {
        state.guardando = false;
        state.errorGuardado = action.error.message;
      })
      .addCase(seleccionarUsuario, () => estadoInicial);
  },
});

export default todosSlice.reducer;
