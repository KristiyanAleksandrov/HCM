import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Link } from 'react-router-dom'
import { Button, TextField, Container, Typography, Box } from '@mui/material'
import { useAuth } from '../contexts/AuthContext'
import { notify } from '../utils/notify'

export default function LoginPage() {
  const { login } = useAuth()
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const navigate = useNavigate()
  const [error, setError] = useState('')

  const handleSubmit = async (e: any) => {
    e.preventDefault()
    try {
      await login(username, password)
      notify('Successful login', 'success')
      navigate('/')
    } catch {
      setError('Invalid credentials')
    }
  }

  return (
    <Container maxWidth="sm">
      <Box sx={{ my: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Login
        </Typography>
        <form onSubmit={handleSubmit}>
          <TextField
            label="Username"
            fullWidth
            margin="normal"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
          <TextField
            label="Password"
            type="password"
            fullWidth
            margin="normal"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          {error && (
            <Typography color="error" variant="body2">
              {error}
            </Typography>
          )}
          <Button type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
            Sign In
          </Button>
        </form>
        <Button component={Link} to="/register" fullWidth sx={{ mt: 1 }}>
          Create Account
        </Button>
      </Box>
    </Container>
  );
}