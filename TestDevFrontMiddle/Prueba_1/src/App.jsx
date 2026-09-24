import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import AppBar from '@mui/material/AppBar';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Container from '@mui/material/Container';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import ListaUsuarios from './components/ListaUsuarios';
import DetalleUsuario from './components/DetalleUsuario';
import { cargarUsuarios, seleccionarUsuarioActual } from './store/usuariosSlice';

export default function App() {
  const dispatch = useDispatch();
  const { estado, error } = useSelector((state) => state.usuarios);
  const usuarioActual = useSelector(seleccionarUsuarioActual);

  useEffect(() => {
    dispatch(cargarUsuarios());
  }, [dispatch]);

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="h1">
            Usuarios, posts y tareas
          </Typography>
        </Toolbar>
      </AppBar>

      <Container sx={{ py: 3 }}>
        {estado === 'cargando' && (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}>
            <CircularProgress />
          </Box>
        )}

        {estado === 'error' && <Alert severity="error">No se pudieron cargar los usuarios: {error}</Alert>}

        {estado === 'listo' && (
          <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' }, gap: 3, alignItems: 'flex-start' }}>
            <Box sx={{ width: { xs: '100%', md: 300 }, flexShrink: 0 }}>
              <ListaUsuarios />
            </Box>

            <Box sx={{ flexGrow: 1, minWidth: 0, width: '100%' }}>
              {usuarioActual ? (
                // El key reinicia la vista (posts/todos) cada vez que se elige otro usuario
                <DetalleUsuario key={usuarioActual.id} usuario={usuarioActual} />
              ) : (
                <Alert severity="info">Selecciona un usuario de la lista para ver su información.</Alert>
              )}
            </Box>
          </Box>
        )}
      </Container>
    </>
  );
}
