import { useState } from 'react';
import { useDispatch } from 'react-redux';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import ListaPosts from './ListaPosts';
import SeccionTodos from './SeccionTodos';
import { cargarPostsDeUsuario } from '../store/postsSlice';
import { cargarTodosDeUsuario } from '../store/todosSlice';

export default function DetalleUsuario({ usuario }) {
  const dispatch = useDispatch();
  const [vista, setVista] = useState(null);

  const mostrarPosts = () => {
    setVista('posts');
    dispatch(cargarPostsDeUsuario(usuario.id));
  };

  const mostrarTodos = () => {
    setVista('todos');
    dispatch(cargarTodosDeUsuario(usuario.id));
  };

  const datos = [
    ['Usuario', usuario.username],
    ['Correo', usuario.email],
    ['Teléfono', usuario.phone],
    ['Sitio web', usuario.website],
    ['Empresa', usuario.company?.name],
    ['Ciudad', usuario.address?.city],
  ];

  return (
    <Stack spacing={3}>
      <Card variant="outlined">
        <CardContent>
          <Typography variant="h5" gutterBottom>
            {usuario.name}
          </Typography>

          {datos.map(([etiqueta, valor]) => (
            <Typography key={etiqueta} variant="body2" color="text.secondary">
              <strong>{etiqueta}:</strong> {valor}
            </Typography>
          ))}

          <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
            <Button variant={vista === 'posts' ? 'contained' : 'outlined'} onClick={mostrarPosts}>
              Posts
            </Button>
            <Button variant={vista === 'todos' ? 'contained' : 'outlined'} onClick={mostrarTodos}>
              Todos
            </Button>
          </Stack>
        </CardContent>
      </Card>

      {vista === 'posts' && <ListaPosts />}
      {vista === 'todos' && <SeccionTodos userId={usuario.id} />}
    </Stack>
  );
}
