import  { useState } from "react";
import { Form, Button, Card, Alert, Spinner } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { loginSchema, type LoginFormData } from "./auth.type";

const LoginPage = () => {
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
      // Simulate authentication - replace with actual API call
      if (data.username === "admin" && data.password === "admin123") {
        // Store token in localStorage (example)
        localStorage.setItem("accessToken", "example-token");
        navigate("/reports/purchases");
      } else {
        throw new Error("Invalid credentials");
      }
    } catch (err) {
      console.error("Login failed:", err);
      setError(err instanceof Error ? err.message : "Invalid username or password");
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
            </Form.Group>
            <Form.Control.Feedback type="invalid">{errors.username?.message}</Form.Control.Feedback>

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

          <div className="text-center mt-3">
            {/* <small className="text-muted">Demo credentials: admin / password</small> */}
          </div>
        </Card.Body>
      </Card>
    </div>
  );
};

export default LoginPage;
