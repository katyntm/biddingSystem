import React, { useState } from 'react';
import { Card, Row, Col, Typography, Button, Tag, Carousel, Image, Statistic, Space } from 'antd';
import { HeartOutlined, ClockCircleOutlined, EyeOutlined, CarOutlined, EnvironmentOutlined } from '@ant-design/icons';
import type { Vehicle } from '../types/vehicle.types';
import { formatCurrency, getTimeRemaining } from '../shared/utils/format';

const { Title, Text } = Typography;

interface VehicleCardProps {
  vehicle: Vehicle;
}

const VehicleCard: React.FC<VehicleCardProps> = ({ vehicle }) => {
  const [isImageVisible, setIsImageVisible] = useState(false);
  const [selectedImage, setSelectedImage] = useState('');
  
  const handleImageClick = (imageUrl: string) => {
    setSelectedImage(imageUrl);
    setIsImageVisible(true);
  };
  
  const timeRemaining = getTimeRemaining(vehicle.endTime);
  const isEnded = new Date(vehicle.endTime) < new Date();

  return (
    <Card 
      className="vehicle-card" 
      bordered={true}
      style={{ marginBottom: 16 }}
    >
      <Row gutter={16}>
        <Col xs={24} sm={8} md={8} lg={7}>
          <div className="vehicle-images">
            <Carousel autoplay={false}>
              {vehicle.images.map((image: string, index: number) => (
                <div key={index} onClick={() => handleImageClick(image)}>
                  <img 
                    src={image} 
                    alt={`${vehicle.year} ${vehicle.make} ${vehicle.model} - Image ${index + 1}`}
                    style={{ width: '100%', height: 200, objectFit: 'cover', cursor: 'pointer' }}
                  />
                </div>
              ))}
            </Carousel>
            
            <div style={{ display: 'flex', marginTop: 8 }}>
              {vehicle.images.slice(0, 3).map((image: string, index: number) => (
                <div 
                  key={index}
                  style={{ 
                    width: 60, 
                    height: 45, 
                    marginRight: 8, 
                    cursor: 'pointer',
                    border: selectedImage === image ? '2px solid #1890ff' : 'none'
                  }}
                  onClick={() => handleImageClick(image)}
                >
                  <img 
                    src={image} 
                    alt={`Thumbnail ${index + 1}`}
                    style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                  />
                </div>
              ))}
              {vehicle.images.length > 3 && (
                <div style={{ width: 60, height: 45, display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#f0f0f0' }}>
                  <Text>+{vehicle.images.length - 3}</Text>
                </div>
              )}
            </div>
          </div>
          
          <Image
            style={{ display: 'none' }}
            preview={{
              visible: isImageVisible,
              onVisibleChange: (visible) => setIsImageVisible(visible),
              src: selectedImage,
            }}
          />
        </Col>
        
        <Col xs={24} sm={16} md={16} lg={12}>
          <div className="vehicle-details">
            <Title level={4} style={{ marginBottom: 8 }}>
              {vehicle.year} {vehicle.make} {vehicle.model} {vehicle.trim}
            </Title>
            
            <div style={{ marginBottom: 16 }}>
              <Space direction="vertical" size={4} style={{ width: '100%' }}>
                <Space>
                  <CarOutlined />
                  <Text>{vehicle.mileage.toLocaleString()} miles</Text>
                </Space>
                <Space>
                  <EnvironmentOutlined />
                  <Text>{vehicle.location}</Text>
                </Space>
                <Text>VIN: <Text strong>{vehicle.vin}</Text></Text>
                <Text>Engine: {vehicle.engineDetails}</Text>
                <Text>Transmission: {vehicle.transmission}</Text>
                <Text>Exterior Color: {vehicle.exteriorColor}</Text>
              </Space>
            </div>
            
            <div style={{ marginTop: 12 }}>
              <Tag color="blue">{vehicle.saleChannel}</Tag>
              <Tag color={isEnded ? 'default' : 'green'}>
                {isEnded ? 'Auction Ended' : 'Active Auction'}
              </Tag>
              {vehicle.numberOfBids > 0 && (
                <Tag color="purple">{vehicle.numberOfBids} bids</Tag>
              )}
            </div>
          </div>
        </Col>
        
        <Col xs={24} sm={24} md={24} lg={5}>
          <div className="vehicle-action" style={{ display: 'flex', flexDirection: 'column', height: '100%', justifyContent: 'space-between' }}>
            <div>
              <Statistic 
                title="Current Price"
                value={vehicle.currentPrice}
                precision={2}
                valueStyle={{ color: '#3f8600', fontWeight: 'bold' }}
                prefix="$"
              />
              
              {vehicle.buyItNowPrice && (
                <Text type="secondary" style={{ display: 'block', marginTop: 4 }}>
                  Buy It Now: {formatCurrency(vehicle.buyItNowPrice)}
                </Text>
              )}
              
              <div style={{ marginTop: 12 }}>
                <Space direction="vertical" size={8} style={{ width: '100%' }}>
                  <div>
                    <ClockCircleOutlined /> 
                    <Text style={{ marginLeft: 8 }}>
                      {isEnded ? 'Ended' : timeRemaining}
                    </Text>
                  </div>
                  
                  <div>
                    <EyeOutlined />
                    <Text style={{ marginLeft: 8 }}>{vehicle.viewCount} views</Text>
                  </div>
                </Space>
              </div>
            </div>
            
            <div style={{ marginTop: 20 }}>
              <Button 
                type="primary" 
                block 
                size="large"
                disabled={isEnded}
                style={{ marginBottom: 8 }}
              >
                {isEnded ? 'Auction Ended' : 'Place Bid'}
              </Button>
              
              <Button 
                block 
                icon={<HeartOutlined />}
              >
                Watch
              </Button>
            </div>
          </div>
        </Col>
      </Row>
    </Card>
  );
};

export default VehicleCard;