# Azure KeyVault Emulator

## Supported Operations

### Keys

#### RSA

- Create Key
- Get Key
- Get Key by Version
- Encrypt
- Decrypt
- Supported [Algorithms](https://docs.microsoft.com/en-us/rest/api/keyvault/decrypt/decrypt#jsonwebkeyencryptionalgorithm)
    - `RSA1_5`
    - `RSA-OAEP`

### Secrets

- Set
- Get Secret
- Get Secret by Version

### Requirements

Azure's KeyClient requires HTTPS communication with a KeyVault instance.
When accessing the emulator on `localhost`, configure a trusted TLS certificate with [dotnet dev-certs](https://docs.microsoft.com/en-us/dotnet/core/additional-tools/self-signed-certificates-guide#with-dotnet-dev-certs).

For accessing the emulator with a hostname other than `localhost`, a self-signed certificate needs to be generated and trusted by the client. See [Adding to docker-compose](#adding-to-docker-compose) for further instructions.

### Adding to docker-compose

For the Azure KeyVault Emulator to be accessible from other containers in the same compose file, a new OpenSSL certificate has to be generated:
1. Replace `<emulator-hostname>` and run the following script to generate a new public/private keypair:

    ```
    openssl req \
    -x509 \
    -newkey rsa:4096 \
    -sha256 \
    -days 3560 \
    -nodes \
    -keyout <emulator-hostname>.key \
    -out <emulator-hostname>.crt \
    -subj '/CN=<emulator-hostname>' \
    -extensions san \
    -config <( \
      echo '[req]'; \
      echo 'distinguished_name=req'; \
      echo '[san]'; \
      echo 'subjectAltName=DNS.1:localhost,DNS.2:<emulator-hostname>')
    ```

1. Export a `.pks` formatted key using the public/private keypair generated in the previous step:

    ```
    openssl pkcs12 -export -out <emulator-hostname>.pfx \
    -inkey <emulator-hostname>.key \
    -in <emulator-hostname>.crt
    ```

1. Trust the certificate in the login keychain

    ```
    sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain <emulator-hostname>.crt
    ```

1. Add a service to docker-compose.yml for Azure KeyVault Emulator:

    ```
    version: '3.7'
    
    services:
      ...
        azure-keyvault-emulator:
        container_name: azure-keyvault-emulator
        image: basis-theory/azure-keyvault-emulator:latest
        ports:
          - 5001:5001
          - 5000:5000
        volumes:
          - <path-to-certs>:/https
        environment:
          - ASPNETCORE_URLS=https://+:5001;http://+:5000
          - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/<emulator-hostname>.pfx
          - KeyVault__Name=<emulator-hostname>
    ```

1. Modify the client application's entrypoint to add the self-signed certificate to the truststore. Example using docker-compose.yml to override the entrypoint:

    ```
    version: '3.7'
    
    services:
      my-awesome-keyvault-client:
        container_name: my-awesome-client
        build:
          context: .
        depends_on:
          - azure-keyvault-emulator
        entrypoint: sh -c "cp /https/<emulator-hostname>.crt /usr/local/share/ca-certificates/<emulator-hostname>.crt && update-ca-certificates && exec <original-entrypoint>"
        volumes:
          - <path-to-certs>:/https
        environment:
          - KeyVault__BaseUrl=https://azure-keyvault-emulator:5001/
    ```

## Development

The provided scripts will check for all dependencies, start docker, build the solution, and run all tests.

### Dependencies
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://www.docker.com/products/docker-desktop)
- [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0)

### Build the KeyVault emulator and run Tests

Run the following command from the root of the project:

```sh
make verify
```
