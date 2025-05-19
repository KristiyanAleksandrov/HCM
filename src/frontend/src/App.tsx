import { Routes, Route, Navigate } from 'react-router-dom'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import PeopleListPage from './pages/PeopleListPage'
import PersonFormPage from './pages/PersonFormPage'
import { useAuth } from './contexts/AuthContext'
import type { ReactNode } from 'react'
import { Box } from '@mui/material'

interface PrivateRouteProps {
  children: ReactNode
}

function PrivateRoute({ children}: PrivateRouteProps) {
  const { token } = useAuth()
  return token ? children : <Navigate to="/login" />
}

export default function App() {
  return (
    <Box
      display="flex"
      justifyContent="center"
      alignItems="center" 
      minHeight="100vh"
      minWidth="100vw"
      bgcolor="#f4f6f8"
    >
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/"
            element={
              <PrivateRoute>
                <PeopleListPage />
              </PrivateRoute>
            }
          />
          <Route
            path="/people/new"
            element={
              <PrivateRoute>
                <PersonFormPage />
              </PrivateRoute>
            }
          />
          <Route
            path="/people/:id"
            element={
              <PrivateRoute>
                <PersonFormPage />
              </PrivateRoute>
            }
          />
        </Routes>
    </Box>
  )
}