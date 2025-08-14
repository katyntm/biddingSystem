import React, { useState, useEffect } from "react";
import { Layout, Row, Col, Pagination, Spin, Card, Select, Input, Button, Typography, Space } from "antd";
import { fetchVehicles } from "../../services/vehicles.service";
import VehicleCard from "../../components/VehicleCard";
import { FilterOutlined } from "@ant-design/icons";
import type { Vehicle, VehicleSearchParams } from "../../types/vehicle.types";

const { Content } = Layout;
const { Title } = Typography;
const { Option } = Select;

const VehiclesPage: React.FC = () => {
  const [searchParams, setSearchParams] = useState<VehicleSearchParams>({ 
    page: 1, 
    pageSize: 12,
    sortBy: 'currentPrice',
    sortOrder: 'asc'
  });
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [totalItems, setTotalItems] = useState(0);
  const [loading, setLoading] = useState(false);
  const [keyword, setKeyword] = useState("");

  useEffect(() => {
    loadVehicles();
  }, [searchParams]);

  const loadVehicles = async () => {
    setLoading(true);
    try {
      const response = await fetchVehicles(searchParams);
      setVehicles(response.items);
      setTotalItems(response.metadata.totalCount);
    } catch (error) {
      console.error("Failed to load vehicles:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    setSearchParams({ ...searchParams, keyword, page: 1 });
  };

  const handleClearFilters = () => {
    setKeyword("");
    setSearchParams({ 
      page: 1, 
      pageSize: 12,
      sortBy: 'currentPrice',
      sortOrder: 'asc'
    });
  };

  const handleSortChange = (value: string) => {
    const [sortBy, sortOrder] = value.split('-');
    setSearchParams({ 
      ...searchParams, 
      sortBy, 
      sortOrder: sortOrder as 'asc' | 'desc',
      page: 1 
    });
  };

  const handlePageChange = (page: number, pageSize?: number) => {
    setSearchParams({ ...searchParams, page, pageSize: pageSize || 12 });
  };

  return (
    <Layout className="vehicles-page">
      {/* <div className="navbar-placeholder" style={{ height: 64, background: "#f0f0f0", marginBottom: 24 }}>
      </div> */}
      
      <Content style={{ padding: "0 50px" }}>
        <div className="search-container" style={{ marginBottom: 24 }}>
          <Row gutter={16} align="top">
            <Col xs={24} md={10} >
              <Card style={{ marginTop: 16 }}>
                <div style={{ marginBottom: 16 }}>
                  <Button 
                    type="primary" 
                    icon={<FilterOutlined />}
                    onClick={handleClearFilters}
                    style={{ marginRight: 16 }}
                  >
                    Clear All Filters
                  </Button>
                  
                  <Input.Search
                    placeholder="Search vehicles"
                    value={keyword}
                    onChange={e => setKeyword(e.target.value)}
                    onSearch={handleSearch}
                    style={{ width: '100%' }}
                  />
                </div>
                
                <div className="filters-section" style={{ height: 200, overflow: 'auto' }}>
                  {/* Filters would be implemented here */}
                  <p>Filters placeholder</p>
                </div>
              </Card>
            </Col>
            
            <Col xs={24} md={14}>
              <Card style={{ marginTop: 16 }}>
                <Row justify="space-between" align="middle" style={{ marginBottom: 16 }}>
                  <Col>
                    <Title level={4} style={{ margin: 0 }}>View Results ({totalItems})</Title>
                  </Col>
                  <Col>
                    <Space>
                      <span>Sort by:</span>
                      <Select 
                        defaultValue="currentPrice-asc" 
                        style={{ width: 180 }}
                        onChange={handleSortChange}
                      >
                        <Option value="currentPrice-asc">Price: Low to High</Option>
                        <Option value="currentPrice-desc">Price: High to Low</Option>
                        <Option value="year-desc">Year: Newest</Option>
                        <Option value="year-asc">Year: Oldest</Option>
                        <Option value="make-asc">Make: A-Z</Option>
                        <Option value="make-desc">Make: Z-A</Option>
                      </Select>
                    </Space>
                  </Col>
                </Row>
                
                {loading ? (
                  <div style={{ textAlign: 'center', padding: '40px 0' }}>
                    <Spin size="large" />
                  </div>
                ) : (
                  <div>
                    <Row gutter={[16, 16]}>
                      {vehicles.map(vehicle => (
                        <Col xs={24} sm={24} md={24} lg={24} xl={24} key={vehicle.id}>
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