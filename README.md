# MasterKafka

MasterKafka là một dự án xử lý Apache Kafka được phát triển một cách chuyên nghiệp bởi HungNV165.
Dự án này là kết quả của sự nghiên cứu sâu sắc và triển khai chất lượng, được thiết kế để đáp ứng các yêu cầu cao cấp về xử lý dữ liệu trên nền tảng Kafka.


## Cài Đặt Kafka bằng docker

Để sử dụng được Kafka cần chạy file docker compose sau:
**Có 3 container cần thiết sau:**   

1. **Zookeeper** - Quản lý Kafka

2. **Kafka** - Kafka Broker

3. **Kafdrop** - Công cụ theo dõi và quản lý cần thiết cho việc load test


```bash
version: '3'
services:
  zookeeper:
    image: wurstmeister/zookeeper
    container_name: zookeeper
    networks:
      - kafka-net
    ports:
      - "2181:2181"

  kafka:
    image: wurstmeister/kafka
    container_name: kafka
    networks:
      - kafka-net
    ports:
      - "9092:9092"
    environment:
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://34.171.40.194:9092
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT
      KAFKA_LISTENERS: PLAINTEXT://0.0.0.0:9092
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181

  kafdrop:
    image: obsidiandynamics/kafdrop
    container_name: kafdrop
    ports:
      - "9091:9000"
    environment:
      KAFKA_BROKERCONNECT: 34.171.40.194:9092
      JVM_OPTS: "-Xms32M -Xmx64M"
    depends_on:
      - kafka

networks:
  kafka-net:
    driver: bridge

```

## Cài Đặt Kafka bằng docker

Để cài đặt Akka.Streams.Kafka, bạn có thể sử dụng NuGet. Chạy lệnh sau trong Package Manager Console:

