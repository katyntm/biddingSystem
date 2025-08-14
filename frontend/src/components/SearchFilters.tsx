import React from "react";
import { Form, Input, Select, Button, Row, Col } from "antd";
import type { VehicleSearchParams } from "../types/vehicle.types";

interface SearchFiltersProps {
  onSearch: (params: Partial<VehicleSearchParams>) => void;
}

const SearchFilters: React.FC<SearchFiltersProps> = ({ onSearch }) => {
  const [form] = Form.useForm();

  const handleSearch = () => {
    form.validateFields().then((values) => {
      onSearch(values);
    });
  };

  const handleReset = () => {
    form.resetFields();
    onSearch({});
  };

  return (
    <Form form={form} layout="vertical">
      <Row gutter={16}>
        <Col span={6}>
          <Form.Item name="make" label="Make">
            <Select placeholder="Select Make">
              <Select.Option value="BMW">BMW</Select.Option>
              <Select.Option value="Ford">Ford</Select.Option>
              <Select.Option value="Chevrolet">Chevrolet</Select.Option>
            </Select>
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="model" label="Model">
            <Input placeholder="Enter Model" />
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="yearFrom" label="Year From">
            <Input type="number" placeholder="Enter Year" />
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="yearTo" label="Year To">
            <Input type="number" placeholder="Enter Year" />
          </Form.Item>
        </Col>
      </Row>
      <Row gutter={16}>
        <Col span={6}>
          <Form.Item name="priceFrom" label="Price From">
            <Input type="number" placeholder="Enter Price" />
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="priceTo" label="Price To">
            <Input type="number" placeholder="Enter Price" />
          </Form.Item>
        </Col>
        <Col span={6}>
          <Form.Item name="saleChannel" label="Sale Channel">
            <Select placeholder="Select Sale Channel">
              <Select.Option value="Live Auction">Live Auction</Select.Option>
              <Select.Option value="Buy It Now">Buy It Now</Select.Option>
            </Select>
          </Form.Item>
        </Col>
        <Col span={6}>
          <Button type="primary" onClick={handleSearch}>
            Search
          </Button>
          <Button onClick={handleReset} style={{ marginLeft: "8px" }}>
            Reset
          </Button>
        </Col>
      </Row>
    </Form>
  );
};

export default SearchFilters;
