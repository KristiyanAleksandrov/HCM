import axios from 'axios'
import { getGlobalLogout } from '../contexts/AuthContext'
import { notify } from '../utils/notify'

const authApi = axios.create({
  baseURL: 'http://localhost:5001'
})

authApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

authApi.interceptors.response.use(
  res => res,
  error => {
    const response = error.response
    if (response?.status === 401) {
      const logout = getGlobalLogout()
      logout()
    }
    if (response?.data) {
      const detail = response.data.detail
      const title = response.data.title
      const errorMessage = detail || title || 'Unexpected error occurred.'
      notify(errorMessage, 'error')
    } else {
      notify('Network error. Please try again later.', 'error')
    }
    return Promise.reject(error)
  }
)

export default authApi;