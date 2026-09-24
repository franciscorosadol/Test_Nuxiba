import { configureStore } from '@reduxjs/toolkit';
import usuariosReducer from './usuariosSlice';
import postsReducer from './postsSlice';
import todosReducer from './todosSlice';

export const store = configureStore({
  reducer: {
    usuarios: usuariosReducer,
    posts: postsReducer,
    todos: todosReducer,
  },
});
