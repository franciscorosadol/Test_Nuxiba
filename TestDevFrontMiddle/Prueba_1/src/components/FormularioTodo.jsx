import { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import FormControlLabel from '@mui/material/FormControlLabel';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { agregarTodo } from '../store/todosSlice';

export default function FormularioTodo({ userId }) {
  const dispatch = useDispatch();
  const { guardando, errorGuardado } = useSelector((state) => state.todos);
  const [title, setTitle] = useState('');
  const [completed, setCompleted] = useState(false);
  const [guardado, setGuardado] = useState(false);

  const guardar = async (evento) => {
    evento.preventDefault();
    setGuardado(false);

    const resultado = await dispatch(agregarTodo({ userId, title: title.trim(), completed }));
    if (agregarTodo.fulfilled.match(resultado)) {
      setTitle('');
      setCompleted(false);
      setGuardado(true);
    }
  };

  return (
    <Paper variant="outlined" component="form" onSubmit={guardar} sx={{ p: 2 }}>
      <Typography variant="h6" gutterBottom>
        Nueva tarea
      </Typography>

      <Stack spacing={2}>
        <TextField
          label="Título"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          required
          fullWidth
        />

        <FormControlLabel
          control={<Checkbox checked={completed} onChange={(e) => setCompleted(e.target.checked)} />}
          label="Completada"
        />

        <Button type="submit" variant="contained" disabled={guardando || title.trim() === ''} sx={{ alignSelf: 'flex-start' }}>
          {guardando ? 'Guardando...' : 'Guardar'}
        </Button>

        {guardado && <Alert severity="success">La tarea se guardó correctamente.</Alert>}
        {errorGuardado && <Alert severity="error">No se pudo guardar la tarea: {errorGuardado}</Alert>}
      </Stack>
    </Paper>
  );
}
