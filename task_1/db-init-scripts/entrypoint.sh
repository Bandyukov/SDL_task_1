#!/bin/bash

psql -v ON_ERROR_STOP=1 -U "postgres" -f "/docker-entrypoint-initdb.d/sql/create_database.sql"
psql -v ON_ERROR_STOP=1 -U "postgres" -d "sdl_task_3" -f "/docker-entrypoint-initdb.d/sql/create_tables.sql"
psql -v ON_ERROR_STOP=1 -U "postgres" -d "sdl_task_3" -f "/docker-entrypoint-initdb.d/sql/create_user.sql"
psql -v ON_ERROR_STOP=1 -U "postgres" -d "sdl_task_3" -f "/docker-entrypoint-initdb.d/sql/create_test_data.sql"