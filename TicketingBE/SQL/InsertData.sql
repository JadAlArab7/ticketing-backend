INSERT INTO tck.department_types (id, name) VALUES ('7a0a3983-d166-4a54-9475-0349a5561867', 'CEO');
INSERT INTO tck.department_types (id, name) VALUES ('afbef9ab-d33a-4099-8eaa-01cd3f31c9dd', 'Head of department');
INSERT INTO tck.department_types (id, name) VALUES ('a28e49ff-a949-4a00-b9ae-f3a1d85f7027', 'Section');

INSERT INTO tck.ticket_types (id, name) VALUES ('ed8415e2-7027-402b-93d3-4b1d6dc970eb', 'RFI');
INSERT INTO tck.ticket_types (id, name) VALUES ('66f12393-b2cc-408a-aeac-d6a3caf18d87', 'Report');

INSERT INTO tck.ticket_status (id, name) VALUES ('9921a92c-c0f7-4c14-95c5-3774e5c3d81b', 'Draft');
INSERT INTO tck.ticket_status (id, name) VALUES ('a5f316aa-7237-4f8a-8014-5f1007075c79', 'Pending');
INSERT INTO tck.ticket_status (id, name) VALUES ('717551bf-5946-4c0e-a025-9c36ee3c549f', 'In progress');
INSERT INTO tck.ticket_status (id, name) VALUES ('d294ae42-fd7d-4a2d-9ff9-829d788e60d0', 'In review');
INSERT INTO tck.ticket_status (id, name) VALUES ('551388b7-4420-4454-bc64-3055be9900bf', 'Completed');
INSERT INTO tck.ticket_status (id, name) VALUES ('45ab3f52-1ff6-491a-8c5a-2af874820f45', 'Need Revision');

INSERT INTO tck.ticket_assignee_type (id, name) VALUES ('e58efe89-2c1d-4102-b328-25ecdfb600dd', 'Assignee');
INSERT INTO tck.ticket_assignee_type (id, name) VALUES ('8e2b116f-94c8-4ac9-aa36-631a23fd3186', 'CC');
select gen_random_uuid(),gen_random_uuid(),gen_random_uuid(),gen_random_uuid(),gen_random_uuid(),gen_random_uuid()

--FOR REPORT
INSERT INTO tck.ticket_status_transitions (id, from_status, to_status, ticket_type_id) 
VALUES ('ed198e1f-4a49-4a7b-8743-ddcce3c77740', '9921a92c-c0f7-4c14-95c5-3774e5c3d81b', 'd294ae42-fd7d-4a2d-9ff9-829d788e60d0', '66f12393-b2cc-408a-aeac-d6a3caf18d87'); --from draft to in review
INSERT INTO tck.ticket_status_transitions (id, from_status, to_status, ticket_type_id) 
VALUES ('392337a9-760e-47b8-9744-934764f5c35b', 'd294ae42-fd7d-4a2d-9ff9-829d788e60d0', '551388b7-4420-4454-bc64-3055be9900bf', '66f12393-b2cc-408a-aeac-d6a3caf18d87'); --from in review to completed
INSERT INTO tck.ticket_status_transitions (id, from_status, to_status, ticket_type_id) 
VALUES ('d1e82ab2-14ee-46a1-8263-e69b2c5a7f33', 'd294ae42-fd7d-4a2d-9ff9-829d788e60d0', '45ab3f52-1ff6-491a-8c5a-2af874820f45', '66f12393-b2cc-408a-aeac-d6a3caf18d87'); --from in review to need revision
INSERT INTO tck.ticket_status_transitions (id, from_status, to_status, ticket_type_id)  
VALUES ('71746036-0683-4204-b22f-a395bbbdb4b6', '45ab3f52-1ff6-491a-8c5a-2af874820f45', 'd294ae42-fd7d-4a2d-9ff9-829d788e60d0', '66f12393-b2cc-408a-aeac-d6a3caf18d87'); --froom need revision to in review

--FOR RFI
INSERT INTO tck.ticket_status_transitions (id, from_status, to_status, ticket_type_id) --from draft to pending
VALUES ('aaef8acf-31b8-4a5c-9d4b-cc86095785e5', '9921a92c-c0f7-4c14-95c5-3774e5c3d81b', 'a5f316aa-7237-4f8a-8014-5f1007075c79', 'ed8415e2-7027-402b-93d3-4b1d6dc970eb'); 
INSERT INTO tck.ticket_status_transitions (id, from_status, to_status, ticket_type_id) --from pending to in progress
VALUES ('aa499663-02d7-46ee-a250-0ffb77391406', 'a5f316aa-7237-4f8a-8014-5f1007075c79', '717551bf-5946-4c0e-a025-9c36ee3c549f', 'ed8415e2-7027-402b-93d3-4b1d6dc970eb'); 
INSERT INTO tck.ticket_status_transitions (id, from_status, to_status, ticket_type_id) --from in progress to in review
VALUES ('4f35ef8a-9d39-4979-ab3d-6aae29e8b5f1', '717551bf-5946-4c0e-a025-9c36ee3c549f', 'd294ae42-fd7d-4a2d-9ff9-829d788e60d0', 'ed8415e2-7027-402b-93d3-4b1d6dc970eb'); 
INSERT INTO tck.ticket_status_transitions (id, from_status, to_status, ticket_type_id) --from in review to completed
VALUES ('acfab215-d47d-4eb6-b9c4-a18bd34b85b7', 'd294ae42-fd7d-4a2d-9ff9-829d788e60d0', '551388b7-4420-4454-bc64-3055be9900bf', 'ed8415e2-7027-402b-93d3-4b1d6dc970eb'); 
INSERT INTO tck.ticket_status_transitions (id, from_status, to_status, ticket_type_id) --from in review to need revision
VALUES ('4ceb50c9-3d7c-45ed-82c2-becf3aed5e8a', 'd294ae42-fd7d-4a2d-9ff9-829d788e60d0', '45ab3f52-1ff6-491a-8c5a-2af874820f45', 'ed8415e2-7027-402b-93d3-4b1d6dc970eb'); 
INSERT INTO tck.ticket_status_transitions (id, from_status, to_status, ticket_type_id) --from need revision to in review
VALUES ('77fca23a-08e8-4b0e-a857-6c0a439169aa', '45ab3f52-1ff6-491a-8c5a-2af874820f45', 'd294ae42-fd7d-4a2d-9ff9-829d788e60d0', 'ed8415e2-7027-402b-93d3-4b1d6dc970eb');