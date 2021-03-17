MAKEFLAGS += --silent

verify:
	docker-compose pull && ./scripts/verify.sh

start-local:
	docker-compose pull && ./scripts/startlocal.sh

build:
	./scripts/build.sh

acceptance-test:
	./scripts/acceptancetest.sh

start-docker:
	./scripts/startdocker.sh

stop-docker:
	./scripts/stopdocker.sh

stop-service:
	./scripts/stopservice.sh
