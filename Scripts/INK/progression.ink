VAR shrine_level = 0           
VAR reputation = 0             
VAR reknown = 0                 
VAR donations_today = 0
VAR daily_visitor_limit = 3

VAR shop_unlocked = false
VAR crafting_unlocked = false

VAR maintenance_state = 25    


== new_day
    ~ donations_today = 0

    ~ temp decay = RANDOM(1,4)
        ~ maintenance_state = maintenance_state - decay
        { maintenance_state < 0:
            ~ maintenance_state = 0
        }

    ~ reputation = maintenance_state

    
    { shrine_level == 0:
        ~ daily_visitor_limit = 3
    }
    { shrine_level == 1:
        ~ daily_visitor_limit = 4
    }
    { shrine_level >= 2:
        ~ daily_visitor_limit = 5
    }

    -> END

== unlock_shrine_level_1
    ~ shrine_level = 1
    ~ shop_unlocked = true
    -> END

== unlock_shrine_level_2
    ~ shrine_level = 2
    ~ crafting_unlocked = true
    -> END

== donate_small
    ~ donations_today += 5
    -> END

== donate_generous
    ~ donations_today += 20
    -> END
