bool_values = [True, False, True, False]

# Convert each boolean to 0 or 1 using int()
int_values = [int(value) for value in bool_values]


print(int_values)  # Output: [1, 0, 1, 0]
