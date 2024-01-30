#import <Foundation/Foundation.h>
#import <mach/mach.h>

extern "C" {
    unsigned int _GetUsedMemorySize() {
        struct task_basic_info basic_info;
        mach_msg_type_number_t t_info_count = TASK_BASIC_INFO_COUNT;
        kern_return_t status;
        
        status = task_info(current_task(), TASK_BASIC_INFO, (task_info_t)&basic_info, &t_info_count);
        
        if (status != KERN_SUCCESS)
        {
            NSLog(@"%s(): Error in task_info(): %s", __FUNCTION__, strerror(errno));
            return -1;
        }
        
        return (unsigned int)basic_info.resident_size;
    }
    
    unsigned int _GetFreeMemorySize() {
        mach_port_t hostPort;
        mach_msg_type_number_t hostSize;
        vm_size_t pagesize;
        
        hostPort = mach_host_self();
        hostSize = sizeof(vm_statistics_data_t) / sizeof(integer_t);
        host_page_size(hostPort, &pagesize);
        vm_statistics_data_t vm_stat;
        
        if (host_statistics(hostPort, HOST_VM_INFO, (host_info_t)&vm_stat, &hostSize) != KERN_SUCCESS) {
            NSLog(@"[SystemMonitor] Failed to fetch vm statistics");
            return -1;
        }
        
        natural_t freeMemory = vm_stat.free_count * pagesize;
        
        return (unsigned int)freeMemory;
    }
    
    unsigned int _GetTotalMemorySize() {
        mach_port_t host_port = mach_host_self();
        mach_msg_type_number_t host_size = sizeof(vm_statistics_data_t) / sizeof(integer_t);
        vm_size_t pagesize;
        vm_statistics_data_t vm_stat;
        
        host_page_size(host_port, &pagesize);
        
        if (host_statistics(host_port, HOST_VM_INFO, (host_info_t)&vm_stat, &host_size) != KERN_SUCCESS) NSLog(@"Failed to fetch vm statistics");
        
        natural_t mem_used = (vm_stat.active_count + vm_stat.inactive_count + vm_stat.wire_count) * pagesize;
        natural_t mem_free = vm_stat.free_count * pagesize;
        natural_t mem_total = mem_used + mem_free;
        
        return (unsigned int)mem_total;
    }
    
    unsigned int _GetPhysicalMemorySize() {
        return (unsigned int)[NSProcessInfo processInfo].physicalMemory;
    }
}
