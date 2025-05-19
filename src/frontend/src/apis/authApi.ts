import axios from 'axios'

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

export default authApi;