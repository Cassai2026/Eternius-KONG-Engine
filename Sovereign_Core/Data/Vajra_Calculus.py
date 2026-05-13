def calculate_vajra_yield(vehicle_count, axle_weight):
    # Kinetic Energy Recovery System (KERS) efficiency for A56
    efficiency = 0.18
    joules = vehicle_count * axle_weight * efficiency
    return joules / 3600000 # Convert to Sovereign kWh
