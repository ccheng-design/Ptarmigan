bool_values = [True, False, True, False]

# Convert each boolean to 0 or 1 using int()
#int_values = [int(value) for value in bool_values]

result=[]

for i in bool_values:
    p=int(i)
    result.append(p)

print(result)

#print(int_values)  # Output: [1, 0, 1, 0]
