int group = 0;  // Initialize the group variable

// Check if the normal of the primitive matches the target normal value
if (v@N.y < 0) {
    group = 1;  // Add primitive to group if the normal is -1 on the Y-axis
}

// Assign the group to the primitive
setprimgroup(0, "normal_minus1", @primnum, group);
