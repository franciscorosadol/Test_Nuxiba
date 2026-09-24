import { useSelector } from 'react-redux';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Checkbox from '@mui/material/Checkbox';
import CircularProgress from '@mui/material/CircularProgress';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import FormularioTodo from './FormularioTodo';

export default function SeccionTodos({ userId }) {
  const { items, estado, error } = useSelector((state) => state.todos);

  return (
    <Stack spacing={2}>
      <FormularioTodo userId={userId} />

      {estado === 'cargando' && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 3 }}>
          <CircularProgress />
        </Box>
      )}

      {estado === 'error' && <Alert severity="error">No se pudieron cargar las tareas: {error}</Alert>}

      {estado === 'listo' && (
        <Box>
          <Typography variant="h6" gutterBottom>
            Tareas ({items.length})
          </Typography>

          <Paper variant="outlined">
            <List disablePadding>
              {items.map((todo) => (
                <ListItem key={todo.id} divider>
                  <ListItemIcon sx={{ minWidth: 40 }}>
                    <Checkbox edge="start" checked={todo.completed} disableRipple readOnly />
                  </ListItemIcon>
                  <ListItemText
                    primary={todo.title}
                    secondary={`#${todo.id}`}
                    sx={{ textDecoration: todo.completed ? 'line-through' : 'none' }}
                  />
                </ListItem>
              ))}
            </List>
          </Paper>
        </Box>
      )}
    </Stack>
  );
}
