import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';

import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { useContexts } from '@/hooks/query/useContexts';
import { CreateContextRequestDto } from '@/types';

interface AddContextModalProps {
  isOpen: boolean;
  onClose: () => void;
}

const formSchema = z.object({
  name: z.string().min(1, { message: 'Dashboard name is required.' }),
  description: z.string().optional(),
});

export function AddContextModal({ isOpen, onClose }: AddContextModalProps) {
  const { createContext, isCreating } = useContexts();

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: '',
      description: '',
    },
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    const newContext: CreateContextRequestDto = {
      name: values.name,
      description: values.description || '',
    };
    createContext(newContext, {
      onSuccess: () => {
        form.reset();
        onClose();
      },
    });
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[425px] bg-surface text-text border-border rounded-xl shadow-2xl">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold text-primary">Create New Dashboard</DialogTitle>
          <DialogDescription className="text-muted-foreground">
            Give your new anime dashboard a name and an optional description.
          </DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="grid gap-4 py-4">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-text">Name</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="My Anime List"
                      {...field}
                      className="bg-background border-border text-text focus-visible:ring-primary"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-text">Description</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="A dashboard for tracking my favorite shonen anime."
                      {...field}
                      className="bg-background border-border text-text focus-visible:ring-primary resize-y min-h-[80px]"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <DialogFooter className="pt-4">
              <Button
                type="submit"
                disabled={isCreating}
                className="bg-primary hover:bg-primary/90 text-primary-foreground rounded-lg shadow-md transition-all duration-200 ease-in-out transform hover:scale-105"
              >
                {isCreating ? 'Creating...' : 'Create Dashboard'}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
