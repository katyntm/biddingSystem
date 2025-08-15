import React, { useState } from "react";
import { Layout, Row, Col, Pagination, Spin, Card, Select, Input, Button, Typography, Space, Alert } from "antd";
import { FilterOutlined } from "@ant-design/icons";
import VehicleCard from "../../components/VehicleCard";
import { useVehicles } from "../../hooks/useVehicles";
import type { VehicleSearchParams } from "../../types/vehicle.types";

const { Content } = Layout;
const { Title } = Typography;
const { Option } = Select;

const VehiclesPage: React.FC = () => {
  const [searchParams, setSearchParams] = useState<VehicleSearchParams>({ 
    page: 1, 
    pageSize: 12,
    sortBy: 'price',
    sortOrder: 'asc'
  });
  const [keyword, setKeyword] = useState("");

  const { data, isLoading, error, isError } = useVehicles(searchParams);

  const vehicles = data?.items || [];
  const totalItems = data?.metadata.totalCount || 0;

  const handleSearch = () => {
    setSearchParams({ ...searchParams, keyword, page: 1 });
  };

  const handleClearFilters = () => {
    setKeyword("");
    setSearchParams({ 
      page: 1, 
      pageSize: 12,
      sortBy: 'price',
      sortOrder: 'asc'
    });
  };

  const handlePageChange = (page: number, pageSize?: number) => {
    setSearchParams({ ...searchParams, page, pageSize: pageSize || 12 });
  };

  if (isError) {
    return (
      <Layout className="vehicles-page">
        <Content style={{ padding: "0 50px" }}>
          <Alert
            message="Error Loading Vehicles"
            description={error?.message || "Failed to load vehicles. Please try again."}
            type="error"
            showIcon
            style={{ margin: "20px 0" }}
          />
        </Content>
      </Layout>
    );
  }

  return (
    <Layout className="vehicles-page">
      <Content style={{ padding: "0 50px" }}>
        <div className="search-container" style={{ marginBottom: 24 }}>
          <Row gutter={16} align="top">
            <Col xs={24} md={8}>
              <Card style={{ marginTop: 16 }}>
                <div style={{ marginBottom: 16 }}>
                  <Button 
                    type="primary" 
                    icon={<FilterOutlined />}
                    onClick={handleClearFilters}
                    style={{ marginRight: 16, marginBottom: 16 }}
                  >
                    Clear All Filters
                  </Button>
                  
                  <Input.Search
                    placeholder="Search vehicles by make, model, or VIN"
                    value={keyword}
                    onChange={e => setKeyword(e.target.value)}
                    onSearch={handleSearch}
                    style={{ width: '100%' }}
                  />
                </div>
                
                <div className="filters-section">
                  {/* <SearchFilters onSearch={handleFilterSearch} /> */}
                </div>
              </Card>
            </Col>
            
            <Col xs={24} md={16}>
              <Card style={{ marginTop: 16 }}>
                <Row justify="space-between" align="middle" style={{ marginBottom: 16 }}>
                  <Col>
                    <Title level={4} style={{ margin: 0 }}>View Results ({totalItems})</Title>
                  </Col>
                  <Col>
                    <Space>
                      <span>Sort by:</span>
                      <Select 
                        defaultValue="price-asc" 
                        style={{ width: 180 }}
                      >
                        <Option value="price-asc">Price: Low to High</Option>
                        <Option value="price-desc">Price: High to Low</Option>
                        <Option value="modelYear-desc">Year: Newest</Option>
                        <Option value="modelYear-asc">Year: Oldest</Option>
                        <Option value="make-asc">Make: A-Z</Option>
                        <Option value="grade-desc">Grade: High to Low</Option>
                      </Select>
                    </Space>
                  </Col>
                </Row>
                
                {isLoading ? (
                  <div style={{ textAlign: 'center', padding: '40px 0' }}>
                    <Spin size="large" />
                  </div>
                ) : (
                  <div>
                    <Row gutter={[16, 16]}>
                      {vehicles.map(vehicle => (
                        <Col xs={24} key={vehicle.id}>
                          <VehicleCard vehicle={vehicle} />
                        </Col>
                      ))}
                    </Row>
                    
                    {vehicles.length === 0 && (
                      <div style={{ textAlign: 'center', padding: '40px 0' }}>
                        <p>No vehicles found matching your criteria</p>
                      </div>
                    )}
                  </div>
                )}
                
                <div style={{ textAlign: 'center', marginTop: 16 }}>
                  <Pagination 
                    current={searchParams.page} 
                    pageSize={searchParams.pageSize}
                    total={totalItems}
                    onChange={handlePageChange}
                    showSizeChanger
                    pageSizeOptions={['12', '24', '36', '48']}
                  />
                </div>
              </Card>
            </Col>
          </Row>
        </div>
      </Content>
    </Layout>
  );
};

export default VehiclesPage;