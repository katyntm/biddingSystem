import { Outlet } from 'react-router-dom';
import { Navbar, Nav, Container, NavDropdown } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import type { User } from '../types/auth.types';

interface MainLayoutProps {
  user: User | null;
  onLogout: () => void;
}

const MainLayout: React.FC<MainLayoutProps> = ({ user, onLogout }) => {
  return (
    <div className="main-layout">
      <Navbar bg="warning" variant="light" expand="lg" className="shadow-sm">
        <Container>
          <Navbar.Brand href="/vehicles">
            <strong>Vehicle Auction</strong>
          </Navbar.Brand>
          
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="ms-auto">
              {/* <LinkContainer to="/vehicles">
                <Nav.Link>Dashboard</Nav.Link>
              </LinkContainer> */}
              <LinkContainer to="/vehicles">
                <Nav.Link>Vehicles</Nav.Link>
              </LinkContainer>
              <LinkContainer to="/reports/bids">
                <Nav.Link>Bid History</Nav.Link>
              </LinkContainer>
              <LinkContainer to="/reports/purchases">
                <Nav.Link>Purchase History</Nav.Link>
              </LinkContainer>
              
              {user && (
                <NavDropdown title={`${user.username} ($${user.balance.toFixed(2)})`} id="user-dropdown">
                  <NavDropdown.Item disabled>
                    {user.email}
                  </NavDropdown.Item>
                  <NavDropdown.Divider />
                  <NavDropdown.Item onClick={onLogout}>
                    Logout
                  </NavDropdown.Item>
                </NavDropdown>
              )}
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>
      
      <main className="flex-grow-1">
        <Outlet />
      </main>
    </div>
  );
};

export default MainLayout;