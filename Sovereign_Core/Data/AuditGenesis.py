import sqlite3
import hashlib

class AuditGenesis:
    def __init__(self):
        self.conn = sqlite3.connect('Sovereign_Core/Data/AuditLedger.db')
        self.create_table()

    def create_table(self):
        query = "CREATE TABLE IF NOT EXISTS evidence_log (id INTEGER PRIMARY KEY, ghost_id TEXT, evidence_hash TEXT, value_recovered REAL)"
        self.conn.execute(query)

    def log_strike(self, ghost_id, evidence_data):
        evidence_hash = hashlib.sha256(evidence_data.encode()).hexdigest()
        self.conn.execute("INSERT INTO evidence_log (ghost_id, evidence_hash, value_recovered) VALUES (?, ?, ?)", 
                          (ghost_id, evidence_hash, 15000.0))
        self.conn.commit()
        print(f"[AUDIT] EVIDENCE SECURED: {evidence_hash}")

if __name__ == "__main__":
    AuditGenesis().log_strike("GHOST_NODE_001", "Stretford Mall Extraction Point 04")
