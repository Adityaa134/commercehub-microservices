import axiosInstance from './axios';
import {
  LoginRequest,
  RegisterRequest,
  RegisterResponse,
  UserProfileResponse,
  EmailRequest,
  MessageResponse,
  ConfirmEmailRequest,
} from '../types/user.types';

export const authService = {
  register: async (data: RegisterRequest): Promise<RegisterResponse> => {
    const response = await axiosInstance.post<RegisterResponse>('/api/auth/register', data);
    return response.data;
  },

  login: async (data: LoginRequest): Promise<UserProfileResponse> => {
    const response = await axiosInstance.post<UserProfileResponse>('/api/auth/login', data);
    return response.data;
  },

  refreshToken: async (): Promise<UserProfileResponse> => {
    const response = await axiosInstance.post<UserProfileResponse>('/api/auth/refresh-token');
    return response.data;
  },

  logout: async (): Promise<void> => {
    await axiosInstance.post('/api/auth/logout');
  },

  forgotPassword: async (data: EmailRequest): Promise<MessageResponse> => {
    const response = await axiosInstance.post<MessageResponse>('/api/auth/forgot-password', data);
    return response.data;
  },

  resendConfirmationEmail: async (data: EmailRequest): Promise<MessageResponse> => {
    const response = await axiosInstance.post<MessageResponse>('/api/auth/resend-confirmation-email', data);
    return response.data;
  },

  confirmEmail: async (data: ConfirmEmailRequest): Promise<MessageResponse> => {
    const response = await axiosInstance.post<MessageResponse>('/api/auth/confirm-email', data);
    return response.data;
  },
};

export default authService;
