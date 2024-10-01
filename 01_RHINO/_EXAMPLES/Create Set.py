# Original list of elements (can contain duplicates)
original_list = ["apple", "banana", "cherry", "apple", "banana", "apple"]

# Create a set to hold unique elements
unique_set = set(original_list)

mapped_index={}
index=0
for string in unique_set:
    mapped_index[string]=index
    index=index+1

mapped_indices=[]
for i in original_list:
    mapped_indices.append(mapped_index[i])


print(mapped_index)
print(unique_set)
print(mapped_indices)

