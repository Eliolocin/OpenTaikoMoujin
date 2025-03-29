from skopt import Optimizer
import numpy as np
from numpy.random import RandomState

# Global optimizer instance
global_optimizer = None

def get_next_delay_to_test(test_data=None):
    # Takes an array of {delay, accuracay} pairs and suggests the next delay to test.

    global global_optimizer
    
    if test_data is None:
        return;
    
    # Extract delays and accuracies from test data
    delays = [pair[0] for pair in test_data]
    accuracies = [pair[1] for pair in test_data]
    
    search_space = [(100, 400)] # Hardcoded for 200, 250, 300 initial data range
    
    # Initialize or update Bayesian Optimizer
    if global_optimizer is None:
        # First time initialization
        global_optimizer = Optimizer(dimensions=search_space, base_estimator="GP", 
                                    random_state=RandomState(42), acq_func="EI")
        # Feed all existing data
        for delay, accuracy in test_data:
            global_optimizer.tell([delay], -accuracy)
    else:
        # Only tell the latest point if it exists
        if len(test_data) > 0:
            latest_delay, latest_accuracy = test_data[-1]
            global_optimizer.tell([latest_delay], -latest_accuracy)
    
    # Get next suggested delay to test
    next_delay = global_optimizer.ask()[0]
    
    # Find current best delay based on existing data
    best_idx = np.argmax(accuracies)
    current_best = delays[best_idx]
    
    return next_delay, current_best, test_data

def run():

    test_data = [ # First 3 initial data to start searching
        (200, 0.30),
        (250, 0.41),
        (300, 0.47)
    ]

    
    while True:
        # Display current test data
        print("\nCurrent test data:")
        print("------------------")
        for i, (delay, accuracy) in enumerate(test_data):
            print(f"{i+1}. Delay: {delay}ms - accuracy: {accuracy}")
        
        # Get next suggested delay
        next_delay, current_best, _ = get_next_delay_to_test(test_data)
        
        print(f"\nCurrent best delay: {current_best}ms with highest accuracy")
        print(f"Suggested next delay to test: {next_delay:.2f}ms")
        
        # Ask user what they want to do
        print("\nChoose an option:")
        print("1. Add new test result")
        print("2. Finish")
        
        choice = input("Enter your choice (1 or 2): ")
        
        if choice == "1":
            # Default to suggested delay
            delay = next_delay
            
            accuracy_input = input(f"Enter the accuracy for {delay}ms (2 decimal places): ")
            try:
                accuracy = round(float(accuracy_input), 2)
                test_data.append((delay, accuracy))
                print(f"Added new data point: Delay {delay}ms - accuracy {accuracy}")
            except ValueError:
                print("Invalid accuracy. Please enter a float.")
        else:
            print("\nFinal recommendation:")
            next_delay, current_best, _ = get_next_delay_to_test(test_data)
            print(f"Best delay found: {current_best}ms")
            break

if __name__ == "__main__":
    run()
