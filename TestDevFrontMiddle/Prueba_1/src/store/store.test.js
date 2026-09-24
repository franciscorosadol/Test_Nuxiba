import { afterEach, describe, expect, it, vi } from 'vitest';
import { configureStore } from '@reduxjs/toolkit';
import usuariosReducer, { cargarUsuarios, seleccionarUsuario } from './usuariosSlice';
import postsReducer, { cargarPostsDeUsuario } from './postsSlice';
import todosReducer, { agregarTodo, cargarTodosDeUsuario } from './todosSlice';

const crearStore = () =>
  configureStore({ reducer: { usuarios: usuariosReducer, posts: postsReducer, todos: todosReducer } });

// Simula fetch respondiendo según la ruta pedida
const simularApi = (rutas) => {
  vi.stubGlobal(
    'fetch',
    vi.fn(async (url) => {
      const ruta = Object.keys(rutas).find((r) => url.endsWith(r));
      return { ok: ruta !== undefined, status: ruta ? 200 : 404, json: async () => rutas[ruta] };
    }),
  );
};

afterEach(() => vi.unstubAllGlobals());

describe('usuarios', () => {
  it('guarda solo los primeros 10 usuarios', async () => {
    simularApi({ '/users': Array.from({ length: 12 }, (_, i) => ({ id: i + 1 })) });
    const store = crearStore();

    await store.dispatch(cargarUsuarios());

    expect(store.getState().usuarios.items).toHaveLength(10);
  });
});

describe('posts', () => {
  it('anida los comentarios dentro de cada post', async () => {
    simularApi({
      '/users/1/posts': [{ id: 7, title: 'a' }],
      '/posts/7/comments': [{ id: 1, postId: 7 }, { id: 2, postId: 7 }],
    });
    const store = crearStore();

    await store.dispatch(cargarPostsDeUsuario(1));

    const [post] = store.getState().posts.items;
    expect(post.comments).toHaveLength(2);
  });

  it('se limpia al seleccionar otro usuario', async () => {
    simularApi({ '/users/1/posts': [{ id: 7 }], '/posts/7/comments': [] });
    const store = crearStore();
    await store.dispatch(cargarPostsDeUsuario(1));

    store.dispatch(seleccionarUsuario(2));

    expect(store.getState().posts.items).toHaveLength(0);
  });
});

describe('todos', () => {
  it('ordena las tareas por id de mayor a menor', async () => {
    simularApi({ '/users/1/todos': [{ id: 2 }, { id: 9 }, { id: 5 }] });
    const store = crearStore();

    await store.dispatch(cargarTodosDeUsuario(1));

    expect(store.getState().todos.items.map((t) => t.id)).toEqual([9, 5, 2]);
  });

  it('nombra las tareas como Tarea 1, Tarea 2... según su orden', async () => {
    simularApi({ '/users/1/todos': [{ id: 1, title: 'delectus' }, { id: 2, title: 'quis ut' }] });
    const store = crearStore();

    await store.dispatch(cargarTodosDeUsuario(1));

    expect(store.getState().todos.items.map((t) => t.title)).toEqual(['Tarea 2', 'Tarea 1']);
  });

  it('envía la nueva tarea con POST y la agrega a la lista', async () => {
    simularApi({ '/users/1/todos': [{ id: 3 }], '/todos': { id: 201 } });
    const store = crearStore();
    await store.dispatch(cargarTodosDeUsuario(1));

    await store.dispatch(agregarTodo({ userId: 1, title: 'nueva', completed: true }));

    const [, opciones] = fetch.mock.calls.at(-1);
    expect(opciones.method).toBe('POST');
    expect(JSON.parse(opciones.body)).toEqual({ userId: 1, title: 'nueva', completed: true });
    expect(store.getState().todos.items[0]).toMatchObject({ id: 201, title: 'nueva' });
  });

  it('no repite el id cuando el API devuelve siempre 201', async () => {
    simularApi({ '/users/1/todos': [{ id: 3 }], '/todos': { id: 201 } });
    const store = crearStore();
    await store.dispatch(cargarTodosDeUsuario(1));

    await store.dispatch(agregarTodo({ userId: 1, title: 'uno', completed: false }));
    await store.dispatch(agregarTodo({ userId: 1, title: 'dos', completed: false }));

    const ids = store.getState().todos.items.map((t) => t.id);
    expect(new Set(ids).size).toBe(ids.length);
  });
});
