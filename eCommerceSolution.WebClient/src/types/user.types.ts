export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  phoneNumber: string;
  password: string;
  confirmPassword: string;
}

export interface RegisterResponse {
  email: string;
  message: string;
}

export interface UserProfileResponse {
  userId: string;
  fullName: string;
  profileImagePath?: string | null;
  role: string;
}

export interface EmailRequest {
  email: string;
}

export interface MessageResponse {
  message: string;
}

export interface ConfirmEmailRequest {
  userId: string;
  token: string;
}

