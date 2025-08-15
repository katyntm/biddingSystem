import React, { useState } from 'react';
import { Card, Row, Col, Typography, Button, Tag, Carousel, Image, Statistic, Space, Rate } from 'antd';
import { ClockCircleOutlined, CarOutlined, EnvironmentOutlined } from '@ant-design/icons';
import { getTimeRemaining, formatCurrency } from '../shared/utils/format';
import type { Vehicle } from '../types/vehicle.types';

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
  
  const timeRemaining = vehicle.endTime ? getTimeRemaining(vehicle.endTime) : 'N/A';
  const isEnded = vehicle.endTime ? new Date(vehicle.endTime) < new Date() : false;

  return (
    <Card 
      className="vehicle-card" 
      // variant='outlined'
      style={{ marginBottom: 16 }}
    >
      <Row gutter={16}>
        <Col xs={24} sm={8} md={8} lg={7}>
          <div className="vehicle-images">
            {vehicle.vehicleImages && vehicle.vehicleImages.length > 0 ? (
              <>
                <Carousel autoplay={false}>
                  {vehicle.vehicleImages.map((image, index) => (
                    <div key={index} onClick={() => handleImageClick(image.url)}>
                      <img 
                        src={image.url} 
                        alt={`${vehicle.modelYear} ${vehicle.make} ${vehicle.modelType} - Image ${index + 1}`}
                        style={{ width: '100%', height: 200, objectFit: 'cover', cursor: 'pointer' }}
                        onError={(e) => {
                          // Fallback to placeholder image if image fails to load
                          const target = e.target as HTMLImageElement;
                          target.src = 'https://images.unsplash.com/photo-1704340142770-b52988e5b6eb?fm=jpg&q=60&w=3000&ixlib=rb-4.1.0&ixid=M3wxMjA3fDF8MHxzZWFyY2h8MXx8Y2FyfGVufDB8fDB8fHww';
                        }}
                      />
                    </div>
                  ))}
                </Carousel>
                
                <div style={{ display: 'flex', marginTop: 8 }}>
                  {vehicle.vehicleImages.slice(0, 3).map((image, index) => (
                    <div 
                      key={index}
                      style={{ 
                        width: 60, 
                        height: 45, 
                        marginRight: 8, 
                        cursor: 'pointer',
                        border: selectedImage === image.url ? '2px solid #1890ff' : 'none'
                      }}
                      onClick={() => handleImageClick(image.url)}
                    >
                      <img 
                        src={image.url} 
                        alt={`Thumbnail ${index + 1}`}
                        style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                        onError={(e) => {
                          const target = e.target as HTMLImageElement;
                          target.src = 'https://images.unsplash.com/photo-1704340142770-b52988e5b6eb?fm=jpg&q=60&w=3000&ixlib=rb-4.1.0&ixid=M3wxMjA3fDF8MHxzZWFyY2h8MXx8Y2FyfGVufDB8fDB8fHww';
                        }}
                      />
                    </div>
                  ))}
                  {vehicle.vehicleImages.length > 3 && (
                    <div style={{ width: 60, height: 45, display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#f0f0f0' }}>
                      <Text>+{vehicle.vehicleImages.length - 3}</Text>
                    </div>
                  )}
                </div>
              </>
            ) : (
              <div style={{ width: '100%', height: 200, background: '#f0f0f0', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <Text>No Images Available</Text>
              </div>
            )}
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
              {vehicle.modelYear} {vehicle.make} {vehicle.modelType}
            </Title>
            
            <div style={{ marginBottom: 16 }}>
              <Space direction="vertical" size={4} style={{ width: '100%' }}>
                <Space>
                  <CarOutlined />
                  <Text>{vehicle.fuelType} • {vehicle.bodyStyle}</Text>
                </Space>
                <Space>
                  <EnvironmentOutlined />
                  <Text>{vehicle.location}</Text>
                </Space>
                <Text>VIN: <Text strong>{vehicle.vin}</Text></Text>
                <Text>Transmission: {vehicle.transmission}</Text>
                <Text>Color: {vehicle.color}</Text>
                <Space>
                  <Text>Grade:</Text>
                  <Rate disabled value={vehicle.grade} style={{ fontSize: 12 }} />
                  <Text>({vehicle.grade}/5)</Text>
                </Space>
              </Space>
            </div>
            
            <div style={{ marginTop: 12 }}>
              {vehicle.saleChannel && <Tag color="blue">{vehicle.saleChannel}</Tag>}
              <Tag color={isEnded ? 'default' : 'green'}>
                {isEnded ? 'Auction Ended' : 'Active Auction'}
              </Tag>
              {vehicle.numberOfBids && vehicle.numberOfBids > 0 && (
                <Tag color="purple">{vehicle.numberOfBids} bids</Tag>
              )}
             
            </div>
          </div>
        </Col>
        
        <Col xs={24} sm={24} md={24} lg={5}>
          <div className="vehicle-action" style={{ display: 'flex', flexDirection: 'column', height: '90%', justifyContent: 'space-between' }}>
            <div>
              <Statistic 
                title="Current Price"
                value={vehicle.price}
                precision={0}
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
                  {vehicle.endTime && (
                    <div>
                      <ClockCircleOutlined /> 
                      <Text style={{ marginLeft: 8 }}>
                        {isEnded ? 'Ended' : timeRemaining}
                      </Text>
                    </div>
                  )}
                  
                  
                </Space>
              </div>
            </div>
            
            <div style={{ marginTop: 10 }}>
              <Button 
                type="primary" 
                block 
                size="large"
                disabled={isEnded || vehicle.isSold}
                style={{ marginBottom: 8 }}
              >
                {isEnded ? 'Auction Ended' : vehicle.isSold ? 'Sold' : 'Place Bid'}
              </Button>
            
            </div>
          </div>
        </Col>
      </Row>
    </Card>
  );
};

export default VehicleCard;