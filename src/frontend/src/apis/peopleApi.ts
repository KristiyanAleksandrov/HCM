import axios from 'axios'

const peopleApi = axios.create({
  baseURL: 'http://localhost:5002'
})

peopleApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default peopleApi;