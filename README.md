# Azure KeyVault Emulator

### Supported Operations

#### RSA

- Create Key
- Get Key by version
- Encrypt
- Decrypt


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
2. Export a `.pks` formatted key using the public/private keypair generated in the previous step:
```
openssl pkcs12 -export -out <emulator-hostname>.pfx \
-inkey <emulator-hostname>.key \
-in <emulator-hostname>.crt
```
3. Add a service to docker-compose.yml for Azure KeyVaultEmulator:
```
version: '3.7'

services:
  ...
    azure-key-vault-emulator:
    container_name: azure-key-vault-emulator
    image: ghcr.io/basis-theory/azure-key-vault-emulator:latest
    ports:
      - 5001:5001
      - 5000:5000
    volumes:
      - <path-to-certs>:/https
    environment:
      - ASPNETCORE_URLS=https://+:5001;http://+:5000
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/<emulator-hostname>.pfx
```
4. Modify the client application's entrypoint to add the self-signed certificate to the truststore. Example using docker-compose.yml to override the entrypoint:
```
version: '3.7'

services:
  my-awesome-key-vault-client:
    container_name: my-awesome-client
    build:
      context: .
    depends_on:
      - azure-key-vault-emulator
    entrypoint: sh -c "cp /https/<emulator-hostname>.crt /usr/local/share/ca-certificates/<emulator-hostname>.crt && update-ca-certificates && exec <original-entrypoint>"
    volumes:
      - <path-to-certs>:/https
    environment:
      - KeyVault__BaseUrl=https://azure-key-vault-emulator:5001/
```
