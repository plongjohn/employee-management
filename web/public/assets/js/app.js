'use strict';

// Status messages after saving or deleting disappear on their own after Bootstrap's default delay.
document.querySelectorAll('.toast').forEach((toast) => bootstrap.Toast.getOrCreateInstance(toast).show());
