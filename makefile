MAKEFLAGS += --silent

verify:
	./scripts/verify.sh

acceptance-test:
	./scripts/acceptancetest.sh

start-docker:
	./scripts/startdocker.sh

stop-docker:
	./scripts/stopdocker.sh

stop-service:
	./scripts/stopservice.sh

release:
	./scripts/release.sh
