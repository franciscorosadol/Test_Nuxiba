import { useDispatch, useSelector } from 'react-redux';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import Paper from '@mui/material/Paper';
import { seleccionarUsuario } from '../store/usuariosSlice';

export default function ListaUsuarios() {
  const dispatch = useDispatch();
  const { items, seleccionadoId } = useSelector((state) => state.usuarios);

  return (
    <Paper variant="outlined">
      <List disablePadding>
        {items.map((usuario) => (
          <ListItemButton
            key={usuario.id}
            selected={usuario.id === seleccionadoId}
            onClick={() => dispatch(seleccionarUsuario(usuario.id))}
          >
            <ListItemText primary={usuario.name} secondary={`@${usuario.username}`} />
          </ListItemButton>
        ))}
      </List>
    </Paper>
  );
}
