import { useState } from "react";
import { Form, Button, Card, Alert, Spinner } from "react-bootstrap";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate } from "react-router-dom";
import { loginSchema, type LoginFormData } from "./auth.type";
import { loginApi } from "../../hooks/useAuth";
import { setAccessToken, setBalance, setEmail, setUserId, setUserName } from "../../shared/utils/auth";
import type { User } from "../../types/auth.types";
import { AxiosError } from "axios";

interface LoginPageProps {
  onLoginSuccess: (user: User) => void;
}

// Error type for API responses
interface ApiErrorResponse {
  message?: string;
  errors?: Record<string, string[]>;
}

const LoginPage: React.FC<LoginPageProps> = ({ onLoginSuccess }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors, isValid },
    reset,
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    mode: "onChange",
  });

  const onSubmit = async (data: LoginFormData) => {
    setError("");
    setLoading(true);

    try {
      const response = await loginApi({
        userName: data.username,
        password: data.password,
      });

      if (response.success) {
        // Store authentication data
        setAccessToken(response.data.token);
        setUserName(response.data.username);
        setEmail(response.data.email);
        setUserId(response.data.userId);
        setBalance(response.data.balance);

        // Create user object for parent component
        const user: User = {
          id: response.data.userId,
          username: response.data.username,
          email: response.data.email,
          balance: response.data.balance,
        };

        // Notify parent component of successful login
        onLoginSuccess(user);
        
        // Navigate to reports page after successful login
        navigate('/reports/bids');
      } else {
        throw new Error(response.message || "Login failed");
      }
    } catch (err: AxiosError<ApiErrorResponse> | Error | unknown) {
      console.error("Login failed:", err);

      let errorMessage = "Invalid username or password";

      if (err instanceof AxiosError) {
        errorMessage = err.response?.data?.message || err.message || "Invalid username or password";
      } else if (err instanceof Error) {
        errorMessage = err.message;
      }

      setError(errorMessage);
      reset({ password: "" });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="d-flex align-items-center justify-content-center vh-100 bg-light">
      <Card className="shadow-sm" style={{ maxWidth: "400px", width: "100%", margin: "0 auto" }}>
        <Card.Body className="p-4">
          <Card.Title className="text-center mb-4 fw-bold fs-4">Vehicle Auction Login</Card.Title>

          {error && <Alert variant="danger">{error}</Alert>}

          <Form onSubmit={handleSubmit(onSubmit)} noValidate>
            <Form.Group className="mb-3" controlId="username">
              <Form.Label>Username</Form.Label>
              <Form.Control type="text" placeholder="Enter username" {...register("username")} isInvalid={!!errors.username} disabled={loading} />
              <Form.Control.Feedback type="invalid">{errors.username?.message}</Form.Control.Feedback>
            </Form.Group>

            <Form.Group className="mb-3" controlId="password">
              <Form.Label>Password</Form.Label>
              <Form.Control type="password" placeholder="Enter password" {...register("password")} isInvalid={!!errors.password} disabled={loading} />
              <Form.Control.Feedback type="invalid">{errors.password?.message}</Form.Control.Feedback>
            </Form.Group>

            <div className="d-grid">
              <Button variant="primary" type="submit" disabled={loading || !isValid} className="d-flex align-items-center justify-content-center">
                {loading && <Spinner as="span" animation="border" size="sm" role="status" className="me-2" />}
                {loading ? "Logging in..." : "Login"}
              </Button>
            </div>
          </Form>
        </Card.Body>
      </Card>
    </div>
  );
};

export default LoginPage;