#import <mach/mach.h>
#import <mach/mach_host.h>
#import <sys/stat.h>

extern "C"
{

    const long igetRam(bool what)
    {
        mach_port_t host_port;
        mach_msg_type_number_t host_size;
        vm_size_t pagesize;

        host_port = mach_host_self();
        host_size = sizeof(vm_statistics_data_t) / sizeof(integer_t);
        host_page_size(host_port, &pagesize);

        vm_statistics_data_t vm_stat;

        if (host_statistics(host_port, HOST_VM_INFO, (host_info_t)&vm_stat, &host_size) != KERN_SUCCESS)
            return -1;

        natural_t used = (vm_stat.active_count + vm_stat.inactive_count + vm_stat.wire_count) * pagesize;
        
        natural_t free = vm_stat.free_count * pagesize;
        
       // natural_t total = used + free;

        if(what)
        return (long) free;
        else
        return (long) used;
    }

    const long GetTotalMemory()
    {
        long allMemory = [[NSProcessInfo processInfo] physicalMemory];
         
        return allMemory;     
    }

    const long GetAvailableMemory()
    {
        vm_statistics_data_t vmStats;
        mach_msg_type_number_t infoCount = HOST_VM_INFO_COUNT;
        kern_return_t kernReturn = host_statistics(mach_host_self(), 
                                                    HOST_VM_INFO, 
                                                    (host_info_t)&vmStats, 
                                                    &infoCount);
          
        if (kernReturn != KERN_SUCCESS) {
            return -1;
        }
          
        return (vm_page_size *vmStats.free_count) ;

    }

     const long GetUsedMemory()
    {
        // vm_size_t pagesize;
        // mach_port_t host_port;
        // mach_msg_type_number_t host_size;

        // host_port = mach_host_self();
        // host_page_size(host_port, &pagesize);

        // vm_statistics_data_t vm_stat;

        // if (host_statistics(host_port, HOST_VM_INFO, (host_info_t)&vm_stat, &host_size) != KERN_SUCCESS)
        //     return -1;

        // return ((vm_stat.active_count + vm_stat.inactive_count + vm_stat.wire_count)* pagesize);

    int64_t memoryUsageInByte = 0;
    task_vm_info_data_t vmInfo;
    mach_msg_type_number_t count = TASK_VM_INFO_COUNT;
    kern_return_t kernelReturn = task_info(mach_task_self(), TASK_VM_INFO, (task_info_t) &vmInfo, &count);
    if(kernelReturn == KERN_SUCCESS) {
        memoryUsageInByte = (int64_t) vmInfo.phys_footprint;
        // NSLog(@"Memory in use (in bytes): %lld", memoryUsageInByte);
    } else {
        // NSLog(@"Error with task_info(): %s", mach_error_string(kernelReturn));
    }
    
    return memoryUsageInByte;

    }

    const long GetResidentMemory()
    {
        task_basic_info_data_t taskInfo;
        mach_msg_type_number_t infoCount = TASK_BASIC_INFO_COUNT;
        kern_return_t kernReturn = task_info(mach_task_self(), 
                                               TASK_BASIC_INFO, 
                                               (task_info_t)&taskInfo, 
                                               &infoCount);
         
        if (kernReturn != KERN_SUCCESS) {
            return NSNotFound;
        }
          
        return taskInfo.resident_size ;
    }


    const long GetActiveMemory()
    {
        mach_port_t host_port;
        vm_size_t pagesize;
        mach_msg_type_number_t host_size;

        host_port = mach_host_self();
        host_page_size(host_port, &pagesize);

        vm_statistics_data_t vm_stat;
        if (host_statistics(host_port, HOST_VM_INFO, (host_info_t)&vm_stat, &host_size) != KERN_SUCCESS)
            return -1;

        return (vm_stat.active_count * pagesize);
    }


    const long GetInactivieMemory()
    {
        mach_port_t host_port;
        vm_size_t pagesize;
        mach_msg_type_number_t host_size;

        host_port = mach_host_self();
        host_page_size(host_port, &pagesize);

        vm_statistics_data_t vm_stat;

        if (host_statistics(host_port, HOST_VM_INFO, (host_info_t)&vm_stat, &host_size) != KERN_SUCCESS)
            return -1;

        return (vm_stat.inactive_count * pagesize);
    }


}