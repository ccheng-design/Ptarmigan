start = 0.0
end = 1.0
step = 0.1

#Create list
result = []

#Rename start to current for the while loop
current = start

#While loop to continue running till it meets the condition
while current < end:
    result.append(round(current, 10))  # Using round to avoid floating-point precision issues
    current += step

print(result)
