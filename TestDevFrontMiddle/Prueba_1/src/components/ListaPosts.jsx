import { useSelector } from 'react-redux';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Divider from '@mui/material/Divider';
import Typography from '@mui/material/Typography';

export default function ListaPosts() {
  const { items, estado, error } = useSelector((state) => state.posts);

  if (estado === 'cargando') {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 3 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (estado === 'error') {
    return <Alert severity="error">No se pudieron cargar los posts: {error}</Alert>;
  }

  if (items.length === 0) {
    return <Alert severity="info">Este usuario no tiene publicaciones.</Alert>;
  }

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Posts ({items.length})
      </Typography>

      {items.map((post) => (
        <Accordion key={post.id} disableGutters variant="outlined">
          <AccordionSummary>
            <Typography sx={{ fontWeight: 500 }}>{post.title}</Typography>
          </AccordionSummary>

          <AccordionDetails>
            <Typography variant="body2" sx={{ mb: 2 }}>
              {post.body}
            </Typography>

            <Typography variant="subtitle2" gutterBottom>
              Comentarios ({post.comments.length})
            </Typography>

            {post.comments.map((comentario) => (
              <Box key={comentario.id} sx={{ mb: 1.5 }}>
                <Divider sx={{ mb: 1 }} />
                <Typography variant="body2" sx={{ fontWeight: 500 }}>
                  {comentario.name}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  {comentario.email}
                </Typography>
                <Typography variant="body2">{comentario.body}</Typography>
              </Box>
            ))}
          </AccordionDetails>
        </Accordion>
      ))}
    </Box>
  );
}
