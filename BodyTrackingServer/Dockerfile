# syntax=docker/dockerfile:1
FROM amazoncorretto:17-alpine3.17-jdk

WORKDIR /

COPY .mvn ./.mvn
COPY mvnw pom.xml ./
COPY src ./src

CMD ["./mvnw","spring-boot:run"]