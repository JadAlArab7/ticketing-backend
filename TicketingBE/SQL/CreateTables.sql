

CREATE TABLE tck.department_types(
	id UUID PRIMARY KEY,
	name TEXT check (name <> '')
);

CREATE TABLE tck.departments(
	id UUID PRIMARY KEY,
	name TEXT check (name <> ''),
	department_type_id UUID NOT NULL REFERENCES tck.department_types(id),
	parent_department_id UUID NULL REFERENCES tck.departments(id)
);

CREATE TABLE tck.users(
	id UUID PRIMARY KEY,
	username TEXT check (username <> ''),
	password TEXT check (password <> ''),
	department_id UUID NOT NULL REFERENCES tck.departments(id)
);

CREATE TABLE tck.ticket_types(
	id UUID PRIMARY KEY,
	name TEXT check (name <> '')
);

CREATE TABLE tck.tickets(
	id UUID PRIMARY KEY,
	ticket_type_id UUID NOT NULL REFERENCES tck.ticket_types(id),
	subject TEXT check (subject <> ''),
	description TEXT check (description <> ''),
	alert_buffer TIMESTAMP,
	deadline TIMESTAMP,
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	created_by UUID NOT NULL REFERENCES tck.departments(id)
);

CREATE TABLE tck.ticket_status(
	id UUID PRIMARY KEY,
	name TEXT check (name <> '')
);

CREATE TABLE tck.ticket_status_transitions(
	id UUID PRIMARY KEY, 
	from_status UUID REFERENCES tck.ticket_status(id),
	to_status UUID REFERENCES tck.ticket_status(id),
	ticket_type_id UUID REFERENCES tck.ticket_types(id)
);

CREATE TABLE tck.ticket_assignee_type(
	id UUID PRIMARY KEY,
	name TEXT check (name <> '')
);

CREATE TABLE tck.ticket_assignees(
	ticket_id UUID REFERENCES tck.tickets(id),
	department_id UUID REFERENCES tck.departments(id),
	ticket_assignee_type UUID REFERENCES tck.ticket_assignee_type(id),
	ticket_status UUID REFERENCES tck.ticket_status(id),
	PRIMARY KEY (ticket_id, department_id)
);

CREATE TABLE tck.ticket_files (
    id UUID PRIMARY KEY,
    file_name TEXT NOT NULL,
    content_type TEXT,
    file_data BYTEA NOT NULL,
	ticket_id UUID REFERENCES tck.tickets(id),
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	created_by UUID NOT NULL REFERENCES tck.departments(id)
);