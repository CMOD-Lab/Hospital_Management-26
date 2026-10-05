-- Minimal seed data for local testing (PostgreSQL / dbo schema)
-- Run after postgresql-schema.sql

INSERT INTO dbo.logintable (password, email, type)
VALUES ('admin', 'admin@clinic.com', 3)
ON CONFLICT (email) DO NOTHING;

INSERT INTO dbo.department (deptno, deptname, description) VALUES
(1, 'Cardiology', 'Heart specialists'),
(2, 'Orthopaedics', 'Musculoskeletal care'),
(3, 'Ears Nose Throat', 'ENT specialists'),
(4, 'Physiotherapy', 'Physical therapy'),
(5, 'Neurology', 'Nervous system disorders')
ON CONFLICT (deptno) DO NOTHING;

INSERT INTO dbo.logintable (password, email, type) VALUES
('abc', 'farhan@gmail.com', 2)
ON CONFLICT (email) DO NOTHING;

INSERT INTO dbo.logintable (password, email, type) VALUES
('abc', 'ABC@gmail.com', 1)
ON CONFLICT (email) DO NOTHING;

INSERT INTO dbo.doctor (
    doctorid, name, phone, address, birthdate, gender,
    deptno, charges_per_visit, monthlysalary, reputeindex,
    patients_treated, qualification, specialization, work_experience, status
)
SELECT loginid, 'Farhan Shoukat', '156133213', 'Enjoy, Lahore', '1996-04-12', 'M',
    1, 2500, 30000, 4, 0, 'PHD IN EVERY FIELD KNOWN TO MAN', 'ENJOY', 10, 1
FROM dbo.logintable WHERE email = 'farhan@gmail.com'
ON CONFLICT (doctorid) DO NOTHING;

INSERT INTO dbo.patient (patientid, name, phone, address, birthdate, gender)
SELECT loginid, 'ABC', '61536516', 'ENJOY, LAHORE', '1996-04-04', 'M'
FROM dbo.logintable WHERE email = 'ABC@gmail.com'
ON CONFLICT (patientid) DO NOTHING;
