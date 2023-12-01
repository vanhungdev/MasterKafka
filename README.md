Producer
A producer publishes messages to Kafka topics. The message itself contains information about what topic and partition to publish to so you can publish to different topics with the same producer.

Settings
When creating a producer stream you need to pass in ProducerSettings that defines things like:

bootstrap servers of the Kafka cluster
serializers for the keys and values
tuning parameters
var producerSettings = ProducerSettings<Null, string>.Create(system, null, null)
    .WithBootstrapServers("localhost:9092");

// OR you can use Config instance
var config = system.Settings.Config.GetConfig("akka.kafka.producer");
var producerSettings = ProducerSettings<Null, string>.Create(config, null, null)
    .WithBootstrapServers("localhost:9092");
NOTE:

Specifying null as a key/value serializer uses default serializer for key/value type. Built-in serializers are available in Confluent.Kafka.Serializers class.